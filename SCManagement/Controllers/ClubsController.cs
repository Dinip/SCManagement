using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Models.Validations;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.ClubService;

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
        private readonly IClubService _clubService;

        /// <summary>
        /// This is the constructor of the Clubs Controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="azureStorage"></param>
        /// <param name="clubService"></param>
        public ClubsController(ApplicationDbContext context, UserManager<User> userManager, IAzureStorage azureStorage, IClubService clubService)
        {
            _context = context;
            _userManager = userManager;
            _azureStorage = azureStorage;
            _clubService = clubService;
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

            //get the club to be viewed
            var club = await _context.Club.Include(c => c.Modalities).Include(c => c.Photography).FirstOrDefaultAsync(m => m.Id == id);

            //If the club does not have an image, one is placed by default.
            if (club.Photography == null)
            {
                club.Photography = new BlobDto { Uri = "https://cdn.scmanagement.me/public/user_placeholder.png" };
            }

            //viewbag that will have the modalities of the club 
            ViewBag.Modalities = new SelectList(club.Modalities, "Id", "Name");

            //get user id
            string userId = GetUserIdFromAuthedUser();

            //get ids of roles that a user have in a club
            List<int> roleIds = UserRolesInClub(userId, (int)id);

            //if the user has a role and this role is not 1(partner), it means that it is staff(general)
            if (roleIds.Count != 0 && !roleIds.Contains(10))
            {
                ViewBag.btnValue = "Tornar-me sócio";
                ViewBag.btnClasses = "btn-primary disabled";
            }
            //Is the user already a member? so stop being
            else if (roleIds.Contains(10))
            {
                //add a associate to the club
                ViewBag.btnValue = "Deixar de ser sócio";
                ViewBag.btnClasses = "btn-danger";
            }
            //the user is not yet a member, so he becomes one (user without roles in the club)
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
            //if (UserAlreadyInAClub(GetUserIdFromAuthedUser())) return NotFound(); //not this, fix

            ViewBag.Modalities = new SelectList(_context.Modality.ToList(), "Id", "Name");

            Address address = new Address();

            Club club = new Club
            {
                Address = address
            };

            return View(club);
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
        public async Task<IActionResult> Create([Bind("Id,Name,ModalitiesIds")] Club club, int CountyId)
        {

            List<Modality> modalities = await _context.Modality.Where(m => club.ModalitiesIds.Contains(m.Id)).ToListAsync();
            if (!ModelState.IsValid) return View();

            //get id of the user
            string userId = GetUserIdFromAuthedUser();

            //check if the user already has/is part of a club and if so, don't allow to create a new one
            //if (UserAlreadyInAClub(userId)) return NotFound(); //not this, fix

            //Create a new club
            Club c = new Club
            {
                Name = club.Name,
                Modalities = modalities,
                CreationDate = DateTime.Now,
                Address = new Address
                {
                    CountyId = CountyId,
                }
            };

            //with this implementation, the user can only create 1 club (1 user per clube atm, might change later)
            List<UsersRoleClub> roles = new();

            //add a role whit the user that create the club
            roles.Add(new UsersRoleClub { UserId = userId, RoleId = 50, JoinDate = DateTime.Now });
            c.UsersRoleClub = roles;

            _context.Club.Add(c);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        // GET: Clubs/Edit/5
        /// <summary>
        /// This method returns the Edit View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Club == null) return NotFound();

            //role id 5 means that is club admin
            if (!UserHasRoleInClub(GetUserIdFromAuthedUser(), (int)id, 50)) return NotFound();

            //get the club
            var club = await _context.Club.Include(c => c.Modalities).Include(c => c.Photography).FirstOrDefaultAsync(c => c.Id == id);

            //get ids of the modalities of the club
            List<int> ClubModalitiesIds = club.Modalities.Select(m => m.Id).ToList();

            //viewbag that have the modalities of the club
            ViewBag.Modalities = new MultiSelectList(_context.Modality.ToList(), "Id", "Name", ClubModalitiesIds);

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
        /// This method returns the Edit View
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

            //get the club
            var actualClub = _context.Club.Include(c => c.Modalities).Include(c => c.Photography).FirstOrDefault(c => c.Id == id);
            if (actualClub == null) return NotFound();

            //delete the club picture 
            if (club.RemoveImage)
            {
                await CheckAndDeletePhoto(actualClub);
            }

            //add club picture
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

            //new modalities choosed
            List<Modality> newModalities = await _context.Modality.Where(m => club.ModalitiesIds.Contains(m.Id)).ToListAsync();

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

            //updates to the settings of club
            actualClub.Name = club.Name;
            actualClub.Email = club.Email;
            actualClub.PhoneNumber = club.PhoneNumber;
            actualClub.About = club.About;

            _context.Club.Update(actualClub);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Allow to know if a user have a role in the club
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


        /// <summary>
        /// Allows to obtain the roles that the user has in the club
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        private List<int> UserRolesInClub(string userId, int clubId)
        {
            List<int> rolesId = new List<int>();
            rolesId.AddRange(_context.UsersRoleClub.Where(f => f.UserId == userId && f.ClubId == clubId).Select(r => r.RoleId).ToList());
            return rolesId;
        }

        /// <summary>
        /// Allow to know if a user are in the club
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get id of the user
        /// </summary>
        /// <returns></returns>
        private string GetUserIdFromAuthedUser()
        {
            return _userManager.GetUserId(User);
        }

        /// <summary>
        /// Allows you to delete a photo that the club had placed
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
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

            [Display(Name = "Modalities")]
            [Required(ErrorMessage = "Error_Required")]
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

            //get club
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

        /// <summary>
        /// Allows you to join as a club member as well as stop being a member
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        private async Task HandleAssociateStatus(int clubId)
        {
            string userId = GetUserIdFromAuthedUser();
            List<int> roleIds = UserRolesInClub(userId, clubId);

            //if the user has a role and this role is not 1(partner), it means that it is staff(general)
            if (roleIds.Count != 0 && !roleIds.Contains(10))
            {
                ViewBag.btnValue = "Associate";
                ViewBag.btnClasses = "btn-primary disabled";
            }
            //Is the user already a member? so stop being
            else if (roleIds.Contains(10))
            {
                //remove a user from a club
                _context.UsersRoleClub.Remove(_context.UsersRoleClub.Where(u => u.UserId == userId && u.ClubId == clubId && u.RoleId == 10).FirstOrDefault());
                await _context.SaveChangesAsync();
                ViewBag.btnValue = "Associate";
                ViewBag.btnClasses = "btn-primary";
            }
            //the user is not yet a member, so he becomes one(user without roles in the club)
            else
            {
                //add a associate to the club
                _context.UsersRoleClub.Add(new UsersRoleClub { UserId = userId, ClubId = clubId, RoleId = 10, JoinDate = DateTime.Now });
                await _context.SaveChangesAsync();
                ViewBag.btnValue = "Member";
                ViewBag.btnClasses = "btn-danger";
            }
        }

        /// <summary>
        /// Return a view which corresponds to the page that has the list of club members
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> PartnersList(int id)
        {
            //get all users of the club that are associate
            if (!UserHasRoleInClub(GetUserIdFromAuthedUser(), (int)id, 50)) return NotFound();

            var associates = _context.UsersRoleClub.Where(u => u.ClubId == id && u.RoleId == 1);

            var partners = await _context.UsersRoleClub.Where(u => u.ClubId == id && u.RoleId == 1).Select(u => u.User).ToListAsync();

            ViewBag.associates = partners;

            return View(associates);
        }

        /// <summary>
        /// Allows to remove a partener from the club
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> RemovePartner(int id)
        {
            var clubId = _context.UsersRoleClub.Where(u => u.Id == id).FirstOrDefault().ClubId;

            //remove a user from a club
            _context.UsersRoleClub.Remove(_context.UsersRoleClub.Where(u => u.Id == id).FirstOrDefault());
            await _context.SaveChangesAsync();

            id = clubId;

            return RedirectToAction("PartnersList", new { id });
        }

        /// <summary>
        /// Return a view which corresponds to the page that has the list of club members
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> MembersList(int id)
        {
            if (id == null) return NotFound();

            string userId = _userManager.GetUserId(User);
            
            //check if the user accessing is manager (secratary or club admin)
            if (!_clubService.IsClubManager(userId, id)) return NotFound();

            ViewBag.ClubId = id;

            //get all members of the club 
            return View(_context.UsersRoleClub.Where(u => u.ClubId == id).Include(u => u.User).Include(u => u.Role).ToList());
        }

        /// <summary>
        /// Allows to remove a member from the club
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> RemoveMember(int id, int clubId)
        {
            //remove a user from a club
            _context.UsersRoleClub.Remove(_context.UsersRoleClub.Where(u => u.Id == id).FirstOrDefault());
            await _context.SaveChangesAsync();

            return RedirectToAction("MembersList", new { id = clubId });
        }



        [Authorize]
        public async Task<IActionResult> CreateCode(int? id)
        {
            if (id == null || _context.Club == null) return NotFound();

            //check if the user accessing is manager (secratary or club admin)
            if (!_clubService.IsClubManager(_userManager.GetUserId(User), (int)id)) return NotFound();

            ViewBag.Roles = new SelectList(await _clubService.GetRoles(), "Id", "RoleName");

            return PartialView("_PartialCreateCode");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCode(int? id, [Bind("RoleId,ExpireDate")] CreateCodeModel codeClub)
        {
            if (id == null || _context.Club == null) return NotFound();

            string userId = _userManager.GetUserId(User);

            //check if the user accessing is manager (secratary or club admin)
            if (!_clubService.IsClubManager(userId, (int)id)) return NotFound();

            if (ModelState.IsValid)
            {
                //generate a code
                DateTime? expireDate = codeClub.ExpireDate;
                if (expireDate != null)
                {
                    expireDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                CodeClub generatedCode = await _clubService.GenerateCode((int)id, userId, codeClub.RoleId, expireDate);
                TempData["Code"] = JsonConvert.SerializeObject(await _clubService.GetCodeWithInfos(generatedCode.Id));

                bool isAdmin = _clubService.IsClubAdmin(userId, (int)id);
                ViewBag.isAdmin = isAdmin;
            }

            return RedirectToAction("Codes", new { id = 1 });
        }

        public class CreateCodeModel
        {
            [Required(ErrorMessage = "Error_Required")]
            [Display(Name = "Role")]
            public int RoleId { get; set; }

            [Display(Name = "Expire Date")]
            [DataType(DataType.Date)]
            public DateTime? ExpireDate { get; set; }
        }

        [Authorize]
        public async Task<IActionResult> Codes(int? id, string? approveCode, int? code)
        {
            if (id == null || _context.Club == null) return NotFound();

            var c = TempData["Code"];
            if (c != null)
            {
                ViewBag.Code = JsonConvert.DeserializeObject<CodeClub>(c.ToString());
            }
            ViewBag.ClubId = id;

            string userId = _userManager.GetUserId(User);

            //check if the user accessing is manager (secratary or club admin)
            if (!_clubService.IsClubManager(userId, (int)id)) return NotFound();
            bool isAdmin = _clubService.IsClubAdmin(userId, (int)id);

            ViewBag.isAdmin = isAdmin;

            if (approveCode != null && isAdmin)
            {
                //approve the usage of the code
                ViewBag.ApprovedCodeStatus = _clubService.ApproveCode(approveCode);
                ViewBag.ApprovedCode = approveCode;
            }

            if (code != null)
            {
                CodeClub? cc = await _clubService.GetCodeWithInfos((int)code);
                if (cc != null && cc.ClubId == id)
                {
                    ViewBag.Code = cc;
                }
            }

            return View(await _clubService.GetCodes((int)id));
        }

        [Authorize]
        public IActionResult Join(string? code)
        {
            return View("Join", new CodeClub { Code = code });
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Join([Bind("Code")] CodeClub cc)
        {
            if (_context.Club == null) return NotFound();

            string userId = _userManager.GetUserId(User);

            KeyValuePair<bool, string> joined = _clubService.UseCode(userId, cc);

            ViewBag.Message = joined.Value;
            if (joined.Key == false) return View("Join", new CodeClub { Code = cc.Code });

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SendCodeEmail(int codeId, string email, int clubId)
        {
            if (_context.Club == null) return NotFound();

            string userId = _userManager.GetUserId(User);

            //check if the user accessing is manager (secratary or club admin)
            if (!_clubService.IsClubManager(userId, clubId)) return NotFound();

            await _clubService.SendCodeEmail(codeId, email, clubId);

            return RedirectToAction("Codes", new { id = 1 });
        }

    }
}
