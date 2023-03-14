using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCManagement.Models;
using SCManagement.Services;
using SCManagement.Services.ClubService;
using SCManagement.Services.PlansService;
using SCManagement.Services.PlansService.Models;
using SCManagement.Services.UserService;
using static SCManagement.Controllers.MyClubController;

namespace SCManagement.Controllers
{
    public class PlansController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;
        private readonly IPlanService _planService;

        public PlansController(
            UserManager<User> userManager,
            IClubService clubService,
            IUserService userService,
            IPlanService planService)
        {
            _userManager = userManager;
            _clubService = clubService;
            _userService = userService;
            _planService = planService;
        }

        private string getUserIdFromAuthedUser()
        {
            //reminder: this does not query the db, it just gets the id from the token (claims)
            return _userManager.GetUserId(User);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(int plan, string athleteId)
        {
            if (plan != 1 && plan != 2) return View("CustomError", "Error_NotFound");

            //Get the user id from the authed user
            var userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            if (plan == 1)
            {
                ViewBag.Modalities = new SelectList(await _clubService.GetModalities(), "Id", "Name");
                var trains = new TrainingPlan()
                {
                    Name = "",
                    Description = "",
                    TrainerId = userId,
                    AthleteId = athleteId,
                    TrainingPlanSessions = new List<TrainingPlanSession>()
                };
                
                trains.TrainingPlanSessions.Add(new TrainingPlanSession() );

                return View("CreateTrainingPlan", trains);
            }
            else if (plan == 2)
            {
                var meals = new MealPlan()
                {
                    TrainerId = userId,
                    AthleteId = athleteId,
                    MealPlanSessions = new List<MealPlanSession>()
                };

                return View("CreateMealPlan", meals);
            }

            return View("CustomError", "Error_NotFound");
        }

        public async Task<IActionResult> GetCreateTrainingPlan([Bind("Name, Description, StartDate, EndDate, TrainerId, AthleteId, ModalityId, TrainingPlanSessions")] TrainingPlan trainingPlan)
        {
            ViewBag.Modalities = new SelectList(await _clubService.GetModalities(), "Id", "Name");
            if (!ModelState.IsValid) return View(trainingPlan);

            if (!CheckIsTrainer()) return View("CustomError", "Error_Unauthorized");

            return View("CreateTrainingPlan", trainingPlan);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainingPlan([Bind("Name, Description, StartDate, EndDate, TrainerId, AthleteId, ModalityId, TrainingPlanSessions")] TrainingPlan trainingPlan)
        {
            ViewBag.Modalities = new SelectList(await _clubService.GetModalities(), "Id", "Name");
            if (!ModelState.IsValid) return View(trainingPlan);

            if (!CheckIsTrainer()) return View("CustomError", "Error_Unauthorized");

            await _planService.CreateTrainingPlan(trainingPlan);

            return RedirectToAction("TrainingSession", "Plans", new { trainingPlan.Id });
        }

        public async Task<IActionResult> TrainingSession(int id)
        {
            if (!CheckIsTrainer()) return View("CustomError", "Error_Unauthorized");
            
            var trainingPlan = await _planService.GetTrainingPlan(id);

            return View("CreateTrainingPlanSession", trainingPlan);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrainingSession()
        {
            if (!CheckIsTrainer()) return View("CustomError", "Error_Unauthorized");

            return RedirectToAction("TrainingZone", "MyClub");
        }

        public async Task<IActionResult> AddTrainingSession(int id)
        {
            if (!CheckIsTrainer()) return View("CustomError", "Error_Unauthorized");

            return PartialView("_PartialAddTrainingPlanSession", new TrainingPlanSession() { TrainingPlanId = id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTrainingSession([Bind("TrainingPlanId, ExerciseName, ExerciseDescription, Repetitions, Duration")] TrainingPlanSession trainingPlanSession)
        {
            if (!ModelState.IsValid) return View(trainingPlanSession);
            
            if (!CheckIsTrainer()) return View("CustomError", "Error_Unauthorized");

            var trainingPlan = await _planService.GetTrainingPlan(trainingPlanSession.TrainingPlanId);
            trainingPlan.TrainingPlanSessions.Add(trainingPlanSession);
            await _planService.UpdateTrainingPlan(trainingPlan);

            return RedirectToAction("TrainingSession", new { id = trainingPlan.Id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMealPlan([Bind("Name, Description, StartDate, EndDate, TrainerId, AthleteId, MealPlanSessions")] MealPlan mealPlan)
        {
            if (!ModelState.IsValid) return View(mealPlan);

            if (!CheckIsTrainer()) return View("CustomError", "Error_Unauthorized");

            await _planService.CreateMealPlan(mealPlan);

            return RedirectToAction("TrainingZone", "MyClub");
        }

        private bool CheckIsTrainer()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = _userService.GetSelectedRole(userId).Result;

            if (!_clubService.IsClubTrainer(role)) return false;

            return true;
        }

    }
}
