using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.AzureStorageService.Models;

namespace SCManagement.Controllers
{
    /// <summary>
    /// This class represents the Clubs Controller
    /// </summary>
    /// 

    public class ClubsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAzureStorage _azureStorage;

        public ClubsController(ApplicationDbContext context, UserManager<User> userManager, IAzureStorage azureStorage)
        {
            _context = context;
            _userManager = userManager;
            _azureStorage = azureStorage;
        }

        // GET: Clubs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Club.Include(c => c.Modalities).ToListAsync());
        }

        // GET: Clubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Club == null)
            {
                return NotFound();
            }

            var club = await _context.Club.Include(c => c.Modalities).Include(c => c.Photography).FirstOrDefaultAsync(m => m.Id == id);
            if (club.Photography == null)
            {
                club.Photography = new BlobDto { Uri = "https://cdn.scmanagement.me/public/user_placeholder.png" };
            }

            //aqui vem logo se o user é ou nao socio (true ou false) dentro da viewbag
            ViewBag.Modalities = new SelectList(club.Modalities, "Id", "Name");

            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        // GET: Clubs/Create
        [Authorize]
        public IActionResult Create()
        {
            //check if the user already has/is part of a club and if so, don't allow to create a new one
            if (UserAlreadyInAClub(GetUserIdFromAuthedUser())) return NotFound(); //not this, fix

            ViewBag.Modalities = new SelectList(_context.Modalities.ToList(), "Id", "Name");
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ModalitiesIds")] Club club)
        {
            List<Modality> modalities = await _context.Modalities.Where(m => club.ModalitiesIds.Contains(m.Id)).ToListAsync();
            if (!ModelState.IsValid) return View();

            string userId = GetUserIdFromAuthedUser();
            //check if the user already has/is part of a club and if so, don't allow to create a new one
            if (UserAlreadyInAClub(userId)) return NotFound(); //not this, fix

            Club c = new Club
            {
                Name = club.Name,
                Modalities = modalities,
                CreationDate = DateTime.Now
            };

            //with this implementation, the user can only create 1 club (1 user per clube atm, might change later)
            List<UsersRoleClub> roles = new();
            roles.Add(new UsersRoleClub { UserId = userId, RoleId = 5 });
            c.UsersRoleClub = roles;

            _context.Club.Add(c);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        // GET: Clubs/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Club == null) return NotFound();

            //role id 5 means that is club admin
            if (!UserHasRoleInClub(GetUserIdFromAuthedUser(), (int)id, 5)) return NotFound();

            var club = await _context.Club.Include(c => c.Modalities).Include(c => c.Photography).FirstOrDefaultAsync(c => c.Id == id);

            List<int> ClubModalitiesIds = club.Modalities.Select(m => m.Id).ToList();

            ViewBag.Modalities = new MultiSelectList(_context.Modalities.ToList(), "Id", "Name", ClubModalitiesIds);

            if (club == null) return NotFound();

            var c = new EditModel
            {
                Id = club.Id,
                Name = club.Name,
                Email = club.Email,
                PhoneNumber = club.PhoneNumber,
                About = club.About,
                CreationDate = club.CreationDate,
                AddressId = club.AddressId,
                Address = club.Address,
                ModalitiesIds = ClubModalitiesIds,
                PhotographyId = club.PhotographyId,
                Photography = club.Photography,
                PhotoUri = club.Photography == null ? "https://cdn.scmanagement.me/public/user_placeholder.png" : club.Photography.Uri,
            };

            return View(c);
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber,About,CreationDate,AddressId,File,RemoveImage,ModalitiesIds")] EditModel club)
        {
            if (id != club.Id) return NotFound();
            if (!ModelState.IsValid) return View(club);
            var actualClub = _context.Club.Include(c => c.Modalities).Include(c => c.Photography).FirstOrDefault(c => c.Id == id);
            if (actualClub == null) return NotFound();


            if (club.RemoveImage)
            {
                await CheckAndDeletePhoto(actualClub);
            }

            if (club.File != null)
            {
                BlobResponseDto uploadResult = await _azureStorage.UploadAsync(club.File);
                if (uploadResult.Error)
                {
                    return RedirectToAction(nameof(Index));
                }
                await CheckAndDeletePhoto(actualClub);
                actualClub.Photography = uploadResult.Blob;
                _context.Club.Update(actualClub);
                await _context.SaveChangesAsync();
            }

            List<Modality> newModalities = await _context.Modalities.Where(m => club.ModalitiesIds.Contains(m.Id)).ToListAsync();
            //remove from club modalities which are not in the new modalities list
            if (actualClub.Modalities != null)
            {
                foreach (Modality m in actualClub.Modalities.ToList())
                {
                    if (!newModalities.Contains(m))
                    {
                        actualClub.Modalities.Remove(m);
                    }
                }

                //add to club modalities the modalities that are in the new modalities list and aren't on club modalities list already
                foreach (Modality m in newModalities.ToList())
                {
                    if (!actualClub.Modalities.Contains(m))
                    {
                        actualClub.Modalities.Add(m);
                    }
                }
            }
            else
            {
                actualClub.Modalities = newModalities;
            }

            actualClub.Name = club.Name;
            actualClub.Email = club.Email;
            actualClub.PhoneNumber = club.PhoneNumber;
            actualClub.About = club.About;

            _context.Club.Update(actualClub);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UserHasRoleInClub(string userId, int clubId, int roleId)
        {
            var role = _context.UsersRoleClub.FirstOrDefault(f => f.UserId == userId && f.ClubId == clubId && f.RoleId == roleId);

            return role != null;
        }

        private bool UserAlreadyInAClub(string userId)
        {
            var alreadyRole = _context.UsersRoleClub.FirstOrDefault(f => f.UserId == userId);
            return alreadyRole != null;
        }

        private string GetUserIdFromAuthedUser()
        {
            return _userManager.GetUserId(User);
        }

        private async Task CheckAndDeletePhoto(Club club)
        {
            if (club.Photography != null)
            {
                await _azureStorage.DeleteAsync(club.Photography.Uuid);
                _context.BlobDto.Remove(club.Photography);
                club.Photography = null;
                //update club to remove photo and save changes
                _context.Update(club);
                await _context.SaveChangesAsync();
            }
        }

        public class EditModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Error_Required")]
            [StringLength(60, ErrorMessage = "Error_Length", MinimumLength = 2)]
            [Display(Name = "Clube Name")]
            public string Name { get; set; }

            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "About")]
            public string? About { get; set; }

            public DateTime CreationDate { get; set; }

            public int? PhotographyId { get; set; }

            public BlobDto? Photography { get; set; }

            public int? AddressId { get; set; }

            public Address? Address { get; set; }

            public IEnumerable<int>? ModalitiesIds { get; set; }

            public string? PhotoUri { get; set; }
            public IFormFile? File { get; set; }
            public bool RemoveImage { get; set; } = false;
        }

        public bool UserWhitoutRoles (int id)
        {
            int countRoles = 5;
            
            for (int i = 0; i < 5; i++)
            {
                if (UserHasRoleInClub(GetUserIdFromAuthedUser(), (int)id, countRoles))
                {
                    return true;
                }
            }
            return false;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Associate(int? id, int clubId)
        {
            //get user
            //var userId = GetUserIdFromAuthedUser();

            //tens que receber o id do clube



            //verify if a user have a role associate in a club, if have remove it, if not add it
            if (UserHasRoleInClub(GetUserIdFromAuthedUser(), (int)id, 1))
            {
                //remove a user from a club
                _context.UsersRoleClub.Remove(_context.UsersRoleClub.Where(u => int.Parse(u.UserId) == id && u.ClubId == clubId).FirstOrDefault());

            }
            else if (!_context.UsersRoleClub.Where(u => int.Parse(u.UserId) == id && u.ClubId == clubId).Any())
            {
                _context.UsersRoleClub.Add(new UsersRoleClub { UserId = ""+id , ClubId = clubId });
            }
            else
            {
                return NotFound();
            }




            return View(); //retorna um 200 ou 500

        }

    }
}
