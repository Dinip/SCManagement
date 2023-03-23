using Microsoft.AspNetCore.Identity;
using System.Security.Policy;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Models;
using SCManagement.Services;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PlansService;
using SCManagement.Services.PlansService.Models;
using SCManagement.Services.TeamService;
using SCManagement.Services.UserService;
using SCManagement.Services.AzureStorageService;

namespace SCManagement.Controllers
{
    public class MyZoneController : Controller
    {
        private readonly ApplicationContextService _applicationContextService;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IPlanService _planService;
        private readonly IClubService _clubService;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly IAzureStorage _azureStorage;
        private readonly UserManager<User> _userManager;

        public MyZoneController(
            ApplicationContextService applicationContextService,
            IUserService userService,
            ITeamService teamService,
            IPlanService planService,
            IClubService clubService,
            IStringLocalizer<SharedResource> stringLocalizer,
            IAzureStorage azureStorage,
            UserManager<User> userManager)
        {
            _applicationContextService = applicationContextService;
            _userService = userService;
            _teamService = teamService;
            _planService = planService;
            _clubService = clubService;
            _stringLocalizer = stringLocalizer;
            _azureStorage = azureStorage;
            _userManager = userManager;
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

            if (role == null || !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            var bio = await _userService.GetBioimpedance(role.UserId);

            var EMDUrl = await PrepareUserEMD(role.UserId);

            var myTeams = await _teamService.GetTeamsByAthlete(role.UserId, role.ClubId);

            var myTrainingPlans = await _planService.GetMyTrainingPlans(role.UserId);
            var myMealPlans = await _planService.GetMyMealPlans(role.UserId);

            var myModel = new MyZoneModel
            {
                UserId = role.UserId,
                EMDUrl = EMDUrl,
                Bioimpedance = bio,
                Teams = myTeams,
                TrainingPlans = myTrainingPlans,
                MealPlans = myMealPlans
            };

            return View(myModel);
        }

        public async Task<IActionResult> CreateBioimpedance()
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");
            var bio = await _userService.GetBioimpedance(role.UserId);
            if (bio != null) return RedirectToAction(nameof(UpdateBioimpedance));

            return View(new Bioimpedance { BioimpedanceId = role.UserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBioimpedance([Bind("BioimpedanceId,Weight,Height,Hydration,FatMass,LeanMass,MuscleMass,ViceralFat,BasalMetabolism")] Bioimpedance bioimpedance)
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            if (!ModelState.IsValid) return View(bioimpedance);

            bioimpedance.BioimpedanceId = role.UserId;

            await _userService.CreateBioimpedance(bioimpedance);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateBioimpedance()
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            var bio = await _userService.GetBioimpedance(role.UserId);

            if (bio == null) return View("CustomError", "Error_DontHaveBioimpedance");

            return View(bio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBioimpedance([Bind("BioimpedanceId,Weight,Height,Hydration,FatMass,LeanMass,MuscleMass,ViceralFat,BasalMetabolism")] Bioimpedance bioimpedance)
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            var bio = await _userService.GetBioimpedance(role.UserId);

            bio.BioimpedanceId = role.UserId;
            bio.Weight = bioimpedance.Weight;
            bio.Height = bioimpedance.Height;
            bio.Hydration = bioimpedance.Hydration;
            bio.FatMass = bioimpedance.FatMass;
            bio.LeanMass = bioimpedance.LeanMass;
            bio.MuscleMass = bioimpedance.MuscleMass;
            bio.ViceralFat = bioimpedance.ViceralFat;
            bio.BasalMetabolism = bioimpedance.BasalMetabolism;

            await _userService.UpdateBioimpedance(bio);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MyZoneEMDUpdate(IFormFile FileEMD)
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (role == null || !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            var user = await _userService.GetUserWithEMD(role.UserId);

            if (FileEMD != null && FileEMD.Length > 0)
            {
                BlobResponseDto uploadResult = await _azureStorage.UploadAsync(FileEMD);
                if (uploadResult.Error)
                {
                    return View("CustomError", "Something wrong");
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
            public Bioimpedance? Bioimpedance { get; set; }
            public IEnumerable<Team>? Teams { get; set; }
            public IEnumerable<TrainingPlan?>? TrainingPlans { get; set; }
            public IEnumerable<MealPlan?>? MealPlans { get; set; }

            public string? EMDUrl { get; set; }
            public bool RemoveEMD { get; set; } = false;
            public IFormFile? FileEMD { get; set; }
        }
    }
}
