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
using SCManagement.Services.Location;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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


        /// <summary>
        /// This method returns the Index View
        /// </summary>
        /// <returns>Index view </returns>
        public async Task<IActionResult> Index()
        {
            return View(await _clubService.GetClubs());
        }


        /// <summary>
        /// This method returns the Details View
        /// </summary>
        /// <param name="id">club id to view</param>
        /// <returns>Deatils view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //get the club to be viewed
            var club = await _clubService.GetClub((int)id);

            if (club == null)
            {
                return NotFound();
            }

            //If the club does not have an image, one is placed by default.
            if (club.Photography == null)
            {
                club.Photography = new BlobDto { Uri = "https://cdn.scmanagement.me/public/user_placeholder.png" };
            }

            //viewbag that will have the modalities of the club 
            ViewBag.Modalities = new SelectList(club.Modalities, "Id", "Name");

            string userId = GetUserIdFromAuthedUser();

            //if the user has a role in the club, cant be partner
            if (_clubService.IsClubMember(userId, club.Id))
            {
                ViewBag.btnValue = "Become partner";
                ViewBag.btnClasses = "btn-primary disabled";
            }
            //Is the user already a member? so stop being
            else if (_clubService.IsClubPartner(userId, club.Id))
            {
                ViewBag.btnValue = "Remove partner";
                ViewBag.btnClasses = "btn-danger";
            }
            //the user is not yet a member, so he becomes one(user without roles in the club)
            else
            {
                ViewBag.btnValue = "Become partner";
                ViewBag.btnClasses = "btn-primary";
            }

            return View(club);
        }


        /// <summary>
        /// This method returns the Create View
        /// </summary>
        /// <returns>Create View</returns>
        [Authorize]
        public IActionResult Create()
        {
            //check if the user already has/is part of a club and if so, don't allow to create a new one
            if (_clubService.UserAlreadyInAClub(GetUserIdFromAuthedUser())) return NotFound(); //not this, fix

            ViewBag.Modalities = new SelectList(_clubService.GetModalities().Result, "Id", "Name");

            return View();
        }


        /// <summary>
        /// This method returns the Create View
        /// </summary>
        /// <param name="club">Clube to be created</param>
        /// <returns>Index View</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ModalitiesIds")] Club club)
        {

            if (!ModelState.IsValid) return View();

            //get id of the user
            string userId = GetUserIdFromAuthedUser();

            //check if the user already has/is part of a club and if so, don't allow to create a new one
            if (_clubService.UserAlreadyInAClub(userId)) return NotFound(); //not this, fix

            await _clubService.CreateClub(club, userId);

            return RedirectToAction(nameof(Index));

        }

        /// <summary>
        /// This method returns the Edit View
        /// </summary>
        /// <param name="id">club id to be edited</param>
        /// <returns>Edit View</returns>
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            //role id 50 means that is club admin
            if (!_clubService.IsClubAdmin(GetUserIdFromAuthedUser(), (int)id)) return NotFound();

            //get the club
            var club = await _clubService.GetClub((int)id);

            //get ids of the modalities of the club
            List<int> ClubModalitiesIds = club.Modalities!.Select(m => m.Id).ToList();

            //viewbag that have the modalities of the club
            ViewBag.Modalities = new MultiSelectList(await _clubService.GetModalities(), "Id", "Name", ClubModalitiesIds);

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


        /// <summary>
        /// This method returns the Edit View
        /// </summary>
        /// <param name="id">id the clube to be edited</param>
        /// <param name="club">club to be edited</param>
        /// <returns>View Index</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Email,PhoneNumber,About,CreationDate,AddressId,File,RemoveImage,ModalitiesIds")] EditModel club)
        {
            if (id == null || id != club.Id) return NotFound();
            if (!ModelState.IsValid) return View(club);

            if (!_clubService.IsClubAdmin(GetUserIdFromAuthedUser(), (int)id)) return NotFound();

            //get the club
            var actualClub = await _clubService.GetClub((int)id);
            if (actualClub == null) return NotFound();

            _clubService.UpdateClubModalities(actualClub, club.ModalitiesIds!);

            //updates to the settings of club
            actualClub.Name = club.Name;
            actualClub.Email = club.Email;
            actualClub.PhoneNumber = club.PhoneNumber;
            actualClub.About = club.About;

            await _clubService.UpdateClubPhoto(actualClub, club.RemoveImage, club.File);

            await _clubService.UpdateClub(actualClub);

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Get id of the user that makes the request
        /// </summary>
        /// <returns>Returns the user id</returns>
        private string GetUserIdFromAuthedUser()
        {
            return _userManager.GetUserId(User);
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
            public IEnumerable<int> ModalitiesIds { get; set; }

            public string? PhotoUri { get; set; }
            public IFormFile? File { get; set; }
            public bool RemoveImage { get; set; } = false;
        }

        /// <summary>
        /// This method allows adding a partner to the club if he has no role in the club, or allows removing a partner from the club if he is.
        /// </summary>
        /// <param name="id">id of the club</param>
        /// <returns>Deatils View</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Associate(int? id)
        {
            if (id == null) return NotFound();

            string userId = GetUserIdFromAuthedUser();
            Club? club = await _clubService.GetClub((int)id);
            if (club == null) return NotFound();

            if (_clubService.IsClubPartner(userId, club.Id))
            {
                //remove a user from a club
                await _clubService.RemoveClubUser(userId, club.Id, 10);
            }
            else if (!_clubService.IsClubMember(userId, club.Id))
            {
                //add a associate to the club
                await _clubService.AddUserToClub(userId, club.Id, 10);
            }

            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Return a view which corresponds to the page that has the list of club members
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns>view PartenersList</returns>
        [Authorize]
        public async Task<IActionResult> PartnersList(int? id)
        {
            if (id == null) return NotFound();
            //get all users of the club that are partner
            if (!_clubService.IsClubManager(GetUserIdFromAuthedUser(), (int)id)) return NotFound();

            return View(await _clubService.GetClubPartners((int)id));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(int? id, string page)
        {
            if (id == null) return NotFound();

            UsersRoleClub? role = await _clubService.GetUserRoleClubFromId((int)id);

            //role with specified id does not exist
            if (role == null) return NotFound();

            string userId = GetUserIdFromAuthedUser();

            if (!_clubService.IsClubManager(userId, role.ClubId)) return NotFound();

            if (role.RoleId == 50) return NotFound();

            //prevent the user secretary from trying to remove another secretary or admin
            if (role.RoleId == 40 && _clubService.IsClubSecretary(userId, role.ClubId)) return NotFound();

            //remove a user from a club
            await _clubService.RemoveClubUser((int)id);

            return RedirectToAction(page, new { id = role.ClubId });
        }

        [Authorize]
        public async Task<IActionResult> StaffList(int? id)
        {
            if (id == null) return NotFound();
            //get all users of the club that are staff members
            if (!_clubService.IsClubManager(GetUserIdFromAuthedUser(), (int)id)) return NotFound();

            return View(await _clubService.GetClubStaff((int)id));
        }

        [Authorize]
        public async Task<IActionResult> CreateCode(int? id)
        {
            if (id == null) return NotFound();

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
            if (id == null) return NotFound();

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
            if (id == null) return NotFound();

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
        public IActionResult Join([Bind("Code")] CodeClub cc)
        {
            string userId = GetUserIdFromAuthedUser();

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
            string userId = GetUserIdFromAuthedUser();

            //check if the user accessing is manager (secratary or club admin)
            if (!_clubService.IsClubManager(userId, clubId)) return NotFound();

            await _clubService.SendCodeEmail(codeId, email, clubId);

            return RedirectToAction("Codes", new { id = 1 });
        }

        [Authorize]
        public async Task<IActionResult> AthletesList(int? id)
        {
            if (id == null) return NotFound();
            //get all users of the club that are athletes
            if (!_clubService.IsClubManager(GetUserIdFromAuthedUser(), (int)id)) return NotFound();

            return View(await _clubService.GetClubAthletes((int)id));
        }
    }
    
}
