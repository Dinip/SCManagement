using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Models;
using SCManagement.Services;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PaymentService;
using SCManagement.Services.PlansService;
using SCManagement.Services.TeamService;
using SCManagement.Services.TranslationService;
using SCManagement.Services.UserService;

namespace SCManagement.Controllers
{
    public class MyZoneController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly ITranslationService _translationService;
        private readonly IPaymentService _paymentService;
        private readonly IPlanService _planService;
        private readonly ApplicationContextService _applicationContextService;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly IAzureStorage _azureStorage;

        /// <summary>
        /// MyZone controller constructor, injects all the services needed
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="clubService"></param>
        /// <param name="userService"></param>
        /// <param name="teamService"></param>
        /// <param name="translationService"></param>
        /// <param name="paymentService"></param>
        /// <param name="applicationContextService"></param>
        /// <param name="stringLocalizer"></param>
        /// <param name="azureStorage"></param>
        /// <param name="planService"></param>
        public MyZoneController(
            UserManager<User> userManager,
            IClubService clubService,
            IUserService userService,
            ITeamService teamService,
            ITranslationService translationService,
            IPaymentService paymentService,
            ApplicationContextService applicationContextService,
            IStringLocalizer<SharedResource> stringLocalizer,
            IAzureStorage azureStorage,
            IPlanService planService)
        {
            _userManager = userManager;
            _clubService = clubService;
            _userService = userService;
            _teamService = teamService;
            _translationService = translationService;
            _paymentService = paymentService;
            _applicationContextService = applicationContextService;
            _stringLocalizer = stringLocalizer;
            _azureStorage = azureStorage;
            _planService = planService;
        }

        private string getUserIdFromAuthedUser()
        {
            //reminder: this does not query the db, it just gets the id from the token (claims)
            return _userManager.GetUserId(User);
        }

        public async Task<IActionResult> Index()
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");

            ViewBag.FileError = TempData["FileError"];

            var EMDUrl = await PrepareUserEMD(role.UserId);

            var myTeams = await _teamService.GetTeamsByAthlete(role.UserId);

            var myModel = new MyZoneModel
            {
                UserId = role.UserId,
                EMDUrl = EMDUrl,
                Teams = myTeams,

            };

            ViewBag.Lang = Thread.CurrentThread.CurrentCulture.Name;

            return View(myModel);
        }

        public async Task<IActionResult> GetTrainingPlans(int? filter)
        {

            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");

            var myTrainingPlans = (await _planService.GetMyTrainingPlans(role.UserId, filter)).ToList();

            myTrainingPlans.Sort((x, y) => x.StartDate.Value.CompareTo(y.StartDate.Value));

            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;

            var obj = myTrainingPlans.Select(p => new
            {
                p.Name,
                p.Description,
                Trainer = p.Trainer.FullName,
                Modality = p.Modality.ModalityTranslations.Where(m => m.Language == cultureInfo).First().Value,
                PlanId = p.Id.ToString(),
                p.StartDate,
                p.EndDate
            });

            return Json(new { data = obj });

        }

        public async Task<IActionResult> GetMealPlans(int? filter)
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");

            var myMealPlans = (await _planService.GetMyMealPlans(role.UserId, filter)).ToList();

            myMealPlans.Sort((x, y) => x.StartDate.Value.CompareTo(y.StartDate.Value));

            var obj = myMealPlans.Select(p => new
            {
                p.Name,
                p.Description,
                Trainer = p.Trainer.FullName,
                PlanId = p.Id,
                p.StartDate,
                p.EndDate
            });

            return Json(new { data = obj });

        }

        public async Task<IActionResult> GetGoals(int? filter)
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");

            var myGoals = (await _planService.GetMyGoals(userId, filter)).ToList();

            myGoals.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            var obj = myGoals.Select(p => new
            {
                p.Name,
                GoalId = p.Id,
                IsCompleted = p.isCompleted,
                p.StartDate,
                p.EndDate
            });

            return Json(new { data = obj });
        }

        public async Task<IActionResult> GetBioimpedance()
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");

            var bios = await _userService.GetBioimpedances(role.UserId);

            return Json(new { data = bios });
        }


        public async Task<IActionResult> CreateBioimpedance()
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");


            var bios = await _userService.GetBioimpedances(role.UserId);
            if (bios != null && bios.Count() != 0) return RedirectToAction(nameof(UpdateBioimpedance));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBioimpedance([Bind("Id,Weight,Height,Hydration,FatMass,LeanMass,MuscleMass,ViceralFat,BasalMetabolism")] BioimpedanceModel bioimpedance)
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");



            if (!ModelState.IsValid) return View(bioimpedance);

            var newBio = new Bioimpedance
            {
                UserId = role.UserId,
                Weight = bioimpedance.Weight,
                Height = bioimpedance.Height,
                Hydration = bioimpedance.Hydration,
                FatMass = bioimpedance.FatMass,
                LeanMass = bioimpedance.LeanMass,
                MuscleMass = bioimpedance.MuscleMass,
                ViceralFat = bioimpedance.ViceralFat,
                BasalMetabolism = bioimpedance.BasalMetabolism,
                LastUpdateDate = DateTime.Now
            };

            await _userService.CreateBioimpedance(newBio);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateBioimpedance()
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");

            var bio = await _userService.GetLastBioimpedance(role.UserId);

            if (bio == null) return View("CustomError", "Error_DontHaveBioimpedance");

            return View(bio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBioimpedance([Bind("Id,Weight,Height,Hydration,FatMass,LeanMass,MuscleMass,ViceralFat,BasalMetabolism")] Bioimpedance bioimpedance)
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");



            var bio = await _userService.GetLastBioimpedance(role.UserId);

            var newBio = new Bioimpedance
            {
                UserId = role.UserId,
                Weight = bioimpedance.Weight,
                Height = bioimpedance.Height,
                Hydration = bioimpedance.Hydration,
                FatMass = bioimpedance.FatMass,
                LeanMass = bioimpedance.LeanMass,
                MuscleMass = bioimpedance.MuscleMass,
                ViceralFat = bioimpedance.ViceralFat,
                BasalMetabolism = bioimpedance.BasalMetabolism,
                LastUpdateDate = DateTime.Now
            };

            await _userService.CreateBioimpedance(newBio);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MyZoneEMDUpdate(IFormFile FileEMD)
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !(await _userService.IsAtleteInAnyClub(role.UserId))) return View("CustomError", "Error_Unauthorized");



            var user = await _userService.GetUserWithEMD(role.UserId);

            if (FileEMD != null && FileEMD.Length > 0)
            {
                BlobResponseDto uploadResult = await _azureStorage.UploadAsync(FileEMD);
                if (uploadResult.Error)
                {
                    if (uploadResult.Status.Contains("10MB"))
                    {
                        TempData["FileError"] = uploadResult.Status;
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View("CustomError", "Something wrong");
                    }
                }
                await _userService.CheckAndDeleteEMD(user);

                user.EMD = uploadResult.Blob;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return View("CustomError", "Something wrong");
                }
            }
            else
            {
                await _userService.CheckAndDeleteEMD(user);
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task<string> PrepareUserEMD(string userId)
        {
            var userWithEMD = await _userService.GetUserWithEMD(userId);
            return userWithEMD.EMD == null ? _stringLocalizer["Pending_Add"] : userWithEMD.EMD.Uri;
        }

        public class MyZoneModel
        {
            public string UserId { get; set; }
            public User? User { get; set; }
            public IEnumerable<Team>? Teams { get; set; }
            public string? EMDUrl { get; set; }
            public bool RemoveEMD { get; set; } = false;
            public IFormFile? FileEMD { get; set; }
        }

        public class BioimpedanceModel
        {
            public int Id { get; set; }
            [Display(Name = "Height")]
            [RegularExpression("^((([1-9][0-9]{0,2})?'?([1-9][0-9]{0,2})(\"([1-9][0-9]{0,2})?|'')?)|([1-9][0-9]{0,2}((\\.|\\,)[0-9]{1,2})?(cm|m)))$",
                ErrorMessage = "Error_Bio_Height")]
            public string? Height { get; set; }

            [Display(Name = "Weight")]
            [RegularExpression("^([1-9][0-9]{0,2}((\\.|\\,)[0-9]{1,2})?(kg|lb|lbs))$",
                ErrorMessage = "Error_Bio_Weight")]
            public string? Weight { get; set; }

            [Display(Name = "FatMass")]
            [Range(0, 100)]
            public float? FatMass { get; set; }

            [Display(Name = "LeanMass")]
            [Range(0, 100)]
            public float? LeanMass { get; set; }

            [Display(Name = "MuscleMass")]
            [Range(0, 100)]
            public float? MuscleMass { get; set; }

            [Display(Name = "ViceralFat")]
            [Range(0, 60)]
            public float? ViceralFat { get; set; }

            [Display(Name = "BasalMetabolism")]
            public float? BasalMetabolism { get; set; }

            [Display(Name = "Hydration")]
            [Range(0, 100)]
            public float? Hydration { get; set; }
        }
    }
}
