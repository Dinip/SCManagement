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

namespace SCManagement.Controllers {
    /// <summary>
    /// This class represents the Clubs Controller
    /// </summary>
    /// 

    public class ClubsController : Controller {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAzureStorage _azureStorage;

        /// <summary>
        /// This is the constructor of the Clubs Controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="azureStorage"></param>
        public ClubsController(ApplicationDbContext context, UserManager<User> userManager, IAzureStorage azureStorage)
        {
            _context = context;
            _userManager = userManager;
            _azureStorage = azureStorage;
        }

        // GET: Clubs
        /// <summary>
        /// This method returns the Index View
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Club.Include(c => c.Modalities).ToListAsync());
        }

        // GET: Clubs/Details/5
        /// <summary>
        /// This method returns the Details View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

            ViewBag.Modalities = new SelectList(club.Modalities, "Id", "Name");

            string userId = GetUserIdFromAuthedUser();
            List<int> roleIds = UserRolesInClub(userId, (int)id);

            //se o user tiver uma role e essa role nao for o 1 (socio), significa que é staff (geral)
            if (roleIds.Count != 0 && !roleIds.Contains(1))
            {
                ViewBag.btnValue = "Tornar-me sócio";
                ViewBag.btnClasses = "btn-primary disabled";
            }
            //o user já é sócio? então deixa de ser
            else if (roleIds.Contains(1))
            {
                //add a associate to the club
                ViewBag.btnValue = "Deixar de ser sócio";
                ViewBag.btnClasses = "btn-danger";
            }
            //o user ainda nao é sócio, então passa a ser (user sem roles no clube)
            else
            {
                //remove a user from a club
                ViewBag.btnValue = "Tornar-me sócio";
                ViewBag.btnClasses = "btn-primary";
            }

            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        // GET: Clubs/Create
        /// <summary>
        /// This method returns the Create View
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// This method returns the Create View
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="club"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private bool UserHasRoleInClub(string userId, int clubId, int roleId)
        {
            var r = _context.UsersRoleClub.FirstOrDefault(f => f.UserId == userId && f.ClubId == clubId && f.RoleId == roleId);
            return r != null;
        }


        private List<int> UserRolesInClub(string userId, int clubId)
        {
            List<int> rolesId = new List<int>();
            rolesId.AddRange(_context.UsersRoleClub.Where(f => f.UserId == userId && f.ClubId == clubId).Select(r => r.RoleId).ToList());
            return rolesId;
        }

        private bool UserAlreadyInAClub(string userId, int? clubId = null)
        {
            if (clubId != null)
            {
                var alreadySpecificRole = _context.UsersRoleClub.FirstOrDefault(f => f.UserId == userId && f.ClubId == clubId);
                return alreadySpecificRole != null;
            }
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

        public class EditModel {
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

        /// <summary>
        /// This method allows adding a member to the club if he has no role in the club, or allows removing a member from the club if he is.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Associate(int? id)
        {
            if (id == null || _context.Club == null)
            {
                return NotFound();
            }

            await HandleAssociateStatus((int)id);

            var club = await _context.Club.Include(c => c.Modalities).Include(c => c.Photography).FirstOrDefaultAsync(m => m.Id == id);
            if (club.Photography == null)
            {
                club.Photography = new BlobDto { Uri = "https://cdn.scmanagement.me/public/user_placeholder.png" };
            }

            ViewBag.Modalities = new SelectList(club.Modalities, "Id", "Name");


            if (club == null)
            {
                return NotFound();
            }

            return View("Details", club);
        }

        private async Task HandleAssociateStatus(int clubId)
        {
            string userId = GetUserIdFromAuthedUser();
            List<int> roleIds = UserRolesInClub(userId, clubId);

            //se o user tiver uma role e essa role nao for o 1 (socio), significa que é staff (geral)
            if (roleIds.Count != 0 && !roleIds.Contains(1))
            {
                ViewBag.btnValue = "Tornar-me sócio";
                ViewBag.btnClasses = "btn-primary disabled";
            }
            //o user já é sócio? então deixa de ser
            else if (roleIds.Contains(1))
            {
                //remove a user from a club
                _context.UsersRoleClub.Remove(_context.UsersRoleClub.Where(u => u.UserId == userId && u.ClubId == clubId && u.RoleId == 1).FirstOrDefault());
                await _context.SaveChangesAsync();
                ViewBag.btnValue = "Tornar-me sócio";
                ViewBag.btnClasses = "btn-primary";
            }
            //o user ainda nao é sócio, então passa a ser (user sem roles no clube)
            else
            {
                //add a associate to the club
                _context.UsersRoleClub.Add(new UsersRoleClub { UserId = userId, ClubId = clubId, RoleId = 1 });
                await _context.SaveChangesAsync();
                ViewBag.btnValue = "Deixar de ser sócio";
                ViewBag.btnClasses = "btn-danger";
            }
        }
    }
}
