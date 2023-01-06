﻿using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using QRCoder;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.UserService;

namespace SCManagement.Controllers
{

    /// <summary>
    /// This class represents the MyClub Controller
    /// </summary>
    /// 
    public class MyClubController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAzureStorage _azureStorage;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;

        /// <summary>
        /// This is the constructor of the MyClub Controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="azureStorage"></param>
        /// <param name="clubService"></param>
        public MyClubController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IAzureStorage azureStorage,
            IClubService clubService,
            IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _azureStorage = azureStorage;
            _clubService = clubService;
            _userService = userService;
        }

        /// <summary>
        /// Get id of the user that makes the request
        /// </summary>
        /// <returns>Returns the user id</returns>
        private string getUserIdFromAuthedUser()
        {
            //reminder: this does not query the db, it just gets the id from the token (claims)
            return _userManager.GetUserId(User);
        }

        /// <summary>
        /// Gets the user selected club (if any)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            if (role.ClubId == 0) return View();

            return View(await _clubService.GetClub(role.ClubId));
        }

        /// <summary>
        /// This method returns the Edit View
        /// </summary>
        /// <param name="id">club id to be edited</param>
        /// <returns>Edit View</returns>
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //checl role
            if (!_clubService.IsClubAdmin(role)) return View("CustomError", "Error_Unauthorized");

            //get the club
            var club = await _clubService.GetClub(role.ClubId);

            //get ids of the modalities of the club
            List<int> ClubModalitiesIds = club.Modalities!.Select(m => m.Id).ToList();

            //viewbag that have the modalities of the club
            ViewBag.Modalities = new MultiSelectList(await _clubService.GetModalities(), "Id", "Name", ClubModalitiesIds);

            if (club == null) return View("CustomError", "Error_NotFound");

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
        public async Task<IActionResult> Edit([Bind("Id,Name,Email,PhoneNumber,About,CreationDate,File,RemoveImage,ModalitiesIds")] EditModel club, int CountyId, string Street, string ZipCode, string Number)
        {
            //check model state
            if (!ModelState.IsValid) return View(club);

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check roles
            if (!_clubService.IsClubAdmin(role)) return View("CustomError", "Error_Unauthorized");

            //check if the club being edited is the same
            //as the one the user has selected
            if (role.ClubId != club.Id) return View("CustomError", "Error_Unauthorized");

            //get club infos
            var actualClub = await _clubService.GetClub(role.ClubId);
            if (actualClub == null) return View("CustomError", "Error_NotFound");

            _clubService.UpdateClubModalities(actualClub, club.ModalitiesIds!);

            //updates to the settings of club
            actualClub.Name = club.Name;
            actualClub.Email = club.Email;
            actualClub.PhoneNumber = club.PhoneNumber;
            actualClub.About = club.About;

            await _clubService.UpdateClubPhoto(actualClub, club.RemoveImage, club.File);

            _clubService.UpdateClubAddress((int)actualClub.AddressId, CountyId, Street, ZipCode, Number);

            await _clubService.UpdateClub(actualClub);

            return RedirectToAction(nameof(Index));
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
        /// Return a view which corresponds to the page that has the list of club members
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns>view PartenersList</returns>
        [Authorize]
        public async Task<IActionResult> PartnersList()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check roles
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.UserRoleId = role.RoleId;

            return View(await _clubService.GetClubPartners(role.ClubId));
        }

        [Authorize]
        public async Task<IActionResult> StaffList()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check roles
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.UserRoleId = role.RoleId;

            return View(await _clubService.GetClubStaff(role.ClubId));
        }

        [Authorize]
        public async Task<IActionResult> AthletesList()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check role
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.UserIsManager = _clubService.IsClubManager(role);

            return View(await _clubService.GetClubAthletes(role.ClubId));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(int? usersRoleClubId, string? page)
        {
            if (usersRoleClubId == null) return View("CustomError", "Error_NotFound");

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            UsersRoleClub? userRoleToBeRomoved = await _clubService.GetUserRoleClubFromId((int)usersRoleClubId);

            //role with specified id does not exist or is club admin (which cant be removed)
            if (userRoleToBeRomoved == null || userRoleToBeRomoved.RoleId == 50) return RedirectToAction(page, new { id = role.ClubId });

            //if the user role to be removed does not belong to the club of the user
            //that is making the request, then return unauthorized
            if (userRoleToBeRomoved.ClubId != role.ClubId) return View("CustomError", "Error_Unauthorized");

            //prevent users that arent club admin from removing club secretary (or higher)
            if (userRoleToBeRomoved.RoleId >= 40 && role.RoleId < 50) return View("CustomError", "Error_NotFound");

            //remove a user(role) from a club
            await _clubService.RemoveClubUser(userRoleToBeRomoved.Id);

            return RedirectToAction(page, new { id = role.ClubId });
        }

        [Authorize]
        public async Task<IActionResult> CreateCode()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.Roles = new SelectList(await _clubService.GetRoles(), "Id", "RoleName");

            return PartialView("_PartialCreateCode");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCode([Bind("RoleId,ExpireDate")] CreateCodeModel codeClub)
        {
            if (!ModelState.IsValid) return RedirectToAction("Codes");

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");


            //generate a code
            DateTime? expireDate = codeClub.ExpireDate;
            if (expireDate != null)
            {
                expireDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            CodeClub codeToBeCreated = new CodeClub
            {
                ClubId = role.ClubId,
                CreatedByUserId = userId,
                RoleId = codeClub.RoleId,
                ExpireDate = expireDate,
                Approved = !(_clubService.IsClubSecretary(role) && codeClub.RoleId >= 40)
            };
            CodeClub generatedCode = await _clubService.GenerateCode(codeToBeCreated);
            TempData["Code"] = JsonConvert.SerializeObject(await _clubService.GetCodeWithInfos(generatedCode.Id));

            ViewBag.isAdmin = _clubService.IsClubAdmin(role);

            return RedirectToAction("Codes");
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

        private void GenerateQRCode(string code)
        {
            using MemoryStream ms = new MemoryStream();
            var baseUri = $"{Request.Scheme}://{Request.Host}:{Request.Host}";
            string link = $"{baseUri}/Clubs/Join/?code={code}";
            QRCodeGenerator qR = new QRCodeGenerator();
            QRCodeData data = qR.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qRCode = new QRCode(data);

            using Bitmap bitmap = qRCode.GetGraphic(20);
            bitmap.Save(ms, format: ImageFormat.Png);
            ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
        }

        [Authorize]
        public async Task<IActionResult> Codes(string? approveCode, int? code)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            var c = TempData["Code"];
            if (c != null)
            {
                CodeClub cc = JsonConvert.DeserializeObject<CodeClub>(c.ToString());
                ViewBag.Code = cc;
                GenerateQRCode(cc.Code);
            }

            ViewBag.isAdmin = _clubService.IsClubAdmin(role);

            if (approveCode != null && _clubService.IsClubAdmin(role))
            {
                //approve the usage of the code
                ViewBag.ApprovedCodeStatus = _clubService.ApproveCode(approveCode);
                ViewBag.ApprovedCode = approveCode;
            }

            if (code != null)
            {
                CodeClub? cc = await _clubService.GetCodeWithInfos((int)code);
                if (cc != null && cc.ClubId == role.ClubId)
                {
                    ViewBag.Code = cc;
                    GenerateQRCode(cc.Code);
                }
            }

            return View(await _clubService.GetCodes(role.ClubId));
        }

        [Authorize]
        public IActionResult Join(string? code)
        {
            return View(new CodeClub { Code = code });
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SendCodeEmail(int codeId, string email)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            await _clubService.SendCodeEmail(codeId, email, role.ClubId);

            return RedirectToAction("Codes", new { id = role.ClubId });
        }
    }
}