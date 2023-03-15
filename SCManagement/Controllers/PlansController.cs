using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PlansService;
using SCManagement.Services.PlansService.Models;
using SCManagement.Services.UserService;

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

        public async Task<IActionResult> TrainingTemplates()
        {   
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");
            
            return View(await _planService.GetTemplateTrainingPlans(userId));
        }

        public async Task<IActionResult> MealTemplates()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();
            
            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");
            
            return View(await _planService.GetTemplateMealPlans(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteTrainingPlan(int? id)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var plan = await _planService.GetTrainingPlan((int)id);
            
            if(plan == null) return View("CustomError", "Error_NotFound");

            if (plan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            await _planService.DeleteTrainingPlan(plan);

            return RedirectToAction(nameof(TrainingTemplates));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteMealPlan(int? id)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var plan = await _planService.GetMealPlan((int)id);

            if (plan == null) return View("CustomError", "Error_NotFound");

            if (plan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            await _planService.DeleteMealPlan(plan);

            return RedirectToAction(nameof(MealTemplates));
        }

        public async Task<IActionResult> CreateTrainingPlan(string? athleteId, string? isTemplate,int? id)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(role.ClubId), "Id", "Name");

            var trains = new TrainingPlan()
            {
                Name = "",
                Description = "",
                TrainerId = getUserIdFromAuthedUser(),
                AthleteId = athleteId,
                TrainingPlanSessions = new List<TrainingPlanSession>() { new TrainingPlanSession() }
            };

            ViewBag.IsTemplate = false;
            if (isTemplate != null) 
            {
                trains.AthleteId = null;
                trains.EndDate = null;
                trains.StartDate = null;
                trains.IsTemplate = true;
                
                ViewBag.IsTemplate = true;
            }

            ViewBag.Apply = false;
            if (id != null)
            {
                var template = await _planService.GetTemplateTrainingPlan((int)id);

                if (template == null) return View("CustomError", "Error_NotFound");
                if (template.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

                trains = new TrainingPlan()
                {
                    Name = template.Name,
                    Description = template.Description,
                    TrainerId = template.TrainerId,
                    AthleteId = athleteId,
                    IsTemplate = false,
                    ModalityId = template.ModalityId,
                    TrainingPlanSessions = template.TrainingPlanSessions.Select(s => new TrainingPlanSession()
                    {
                        ExerciseName = s.ExerciseName,
                        ExerciseDescription = s.ExerciseDescription,
                        Duration = s.Duration,
                        Repetitions = s.Repetitions,
                    }).ToList()
                };

                ViewBag.Apply = true;
            }
            
            return View("CreateTrainingPlan", trains);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainingPlan([Bind("Name, Description, StartDate, EndDate, TrainerId, AthleteId, ModalityId, IsTemplate, TrainingPlanSessions")] TrainingPlan trainingPlan, string action, string? apply)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(role.ClubId), "Id", "Name");
            ViewBag.IsTemplate = trainingPlan.IsTemplate;
            ViewBag.Apply = Convert.ToBoolean(apply);

            if (action == "Add sessions")
            {
                var skipped = ModelState.Keys;
                foreach (var key in skipped)
                    ModelState[key].Errors.Clear();

                if (trainingPlan.TrainingPlanSessions == null) { trainingPlan.TrainingPlanSessions = new List<TrainingPlanSession>(); }
                trainingPlan.TrainingPlanSessions.Add(new TrainingPlanSession());

                return View("CreateTrainingPlan", trainingPlan);
            }
            else if (action == "Create" || action == "Apply")
            {
                if (!ModelState.IsValid) return View(trainingPlan);

                await _planService.CreateTrainingPlan(trainingPlan);

                if (trainingPlan.IsTemplate) return RedirectToAction("TrainingTemplates");

                return RedirectToAction("TrainingZone", "MyClub");
            }
            
            return View("CustomError", "Error_NotFound");
        }

        public async Task<IActionResult> CreateMealPlan(string? athleteId, string? isTemplate, int? id)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var meals = new MealPlan()
            {
                Name = "",
                Description = "",
                TrainerId = getUserIdFromAuthedUser(),
                AthleteId = athleteId,
                MealPlanSessions = new List<MealPlanSession>() { new MealPlanSession() }
            };

            ViewBag.IsTemplate = false;
            if (isTemplate != null)
            {
                meals.AthleteId = null;
                meals.EndDate = null;
                meals.StartDate = null;
                meals.IsTemplate = true;

                ViewBag.IsTemplate = true;
            }

            ViewBag.Apply = false;
            if (id != null)
            {
                var template = await _planService.GetTemplateMealPlan((int)id);

                if (template == null) return View("CustomError", "Error_NotFound");
                if (template.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

                meals = new MealPlan()
                {
                    Name = template.Name,
                    Description = template.Description,
                    TrainerId = template.TrainerId,
                    AthleteId = athleteId,
                    IsTemplate = false,
                    MealPlanSessions = template.MealPlanSessions.Select(s => new MealPlanSession()
                    {
                        MealName = s.MealName,
                        MealDescription = s.MealDescription,
                        Time = s.Time,
                    }).ToList()
                };

                ViewBag.Apply = true;
            }


            return View("CreateMealPlan", meals);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMealPlan([Bind("Name, Description, StartDate, EndDate, TrainerId, AthleteId, IsTemplate, MealPlanSessions")] MealPlan mealPlan, string action, string? apply)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.IsTemplate = mealPlan.IsTemplate;
            ViewBag.Apply = Convert.ToBoolean(apply);

            if (action == "Add sessions")
            {
                var skipped = ModelState.Keys;
                foreach (var key in skipped)
                    ModelState[key].Errors.Clear();

                if (mealPlan.MealPlanSessions == null) { mealPlan.MealPlanSessions = new List<MealPlanSession>(); }
                mealPlan.MealPlanSessions.Add(new MealPlanSession());

                return View("CreateMealPlan", mealPlan);
            }
            else if (action == "Create" || action == "Apply")
            {
                if (!ModelState.IsValid) return View(mealPlan);

                await _planService.CreateMealPlan(mealPlan);

                if(mealPlan.IsTemplate) return RedirectToAction("MealTemplates");

                return RedirectToAction("TrainingZone", "MyClub");
            }

            return View("CustomError", "Error_NotFound");
        }
        
        public async Task<IActionResult> EditTrainingPlan(int? id)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var trainingPlan = await _planService.GetTrainingPlan((int)id);

            if (trainingPlan == null) return View("CustomError", "Error_NotFound");

            if (trainingPlan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(role.ClubId), "Id", "Name");
            ViewBag.IsTemplate = trainingPlan.IsTemplate;
            
            return View(trainingPlan);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainingPlan([Bind("Id, Name, Description, StartDate, EndDate, TrainerId, AthleteId, ModalityId, IsTemplate, TrainingPlanSessions")] TrainingPlan trainingPlan, string action)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            if (trainingPlan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(role.ClubId), "Id", "Name");
            ViewBag.IsTemplate = trainingPlan.IsTemplate;

            if (action == "Add sessions")
            {
                var skipped = ModelState.Keys;
                foreach (var key in skipped)
                    ModelState[key].Errors.Clear();

                if (trainingPlan.TrainingPlanSessions == null) { trainingPlan.TrainingPlanSessions = new List<TrainingPlanSession>(); }
                trainingPlan.TrainingPlanSessions.Add(new TrainingPlanSession());

                return View("EditTrainingPlan", trainingPlan);
            }
            else if (action == "Edit")
            {
                if (!ModelState.IsValid) return View(trainingPlan);

                var actualTrainingPlan = await _planService.GetTrainingPlan(trainingPlan.Id);

                if (actualTrainingPlan == null) return View("CustomError", "Error_NotFound");

                if (trainingPlan.TrainerId != actualTrainingPlan.TrainerId) return View("CustomError", "Error_Unauthorized");

                if (trainingPlan.TrainingPlanSessions == null) { trainingPlan.TrainingPlanSessions = new List<TrainingPlanSession>(); }

                //remove from Plan the sessions which are not in the new sessions list
                foreach (TrainingPlanSession p in actualTrainingPlan.TrainingPlanSessions!)
                {
                    if (!trainingPlan.TrainingPlanSessions!.Contains(p))
                    {
                        actualTrainingPlan.TrainingPlanSessions.Remove(p);
                    }
                }

                //add to Plan sessions the sessions that are in the new sessions list and aren't on Plan sessions list already
                foreach (TrainingPlanSession p in trainingPlan.TrainingPlanSessions!)
                {
                    if (!actualTrainingPlan.TrainingPlanSessions!.Contains(p))
                    {
                        actualTrainingPlan.TrainingPlanSessions.Add(p);
                    }
                }

                actualTrainingPlan.Name = trainingPlan.Name;
                actualTrainingPlan.Description = trainingPlan.Description;
                actualTrainingPlan.ModalityId = trainingPlan.ModalityId;

                if (!actualTrainingPlan.IsTemplate)
                {
                    actualTrainingPlan.StartDate = trainingPlan.StartDate;
                    actualTrainingPlan.EndDate = trainingPlan.EndDate;
                }

                await _planService.UpdateTrainingPlan(actualTrainingPlan);

                return RedirectToAction("TrainingTemplates");
            }

            return View("CustomError", "Error_NotFound");
        }

        public async Task<IActionResult> EditMealPlan(int? id)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var mealPlan = await _planService.GetMealPlan((int)id);

            if (mealPlan == null) return View("CustomError", "Error_NotFound");

            if (mealPlan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");
            
            ViewBag.IsTemplate = mealPlan.IsTemplate;

            return View(mealPlan);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMealPlan([Bind("Id, Name, Description, StartDate, EndDate, TrainerId, AthleteId, IsTemplate, MealPlanSessions")] MealPlan mealPlan, string action)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            if (mealPlan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(role.ClubId), "Id", "Name");
            ViewBag.IsTemplate = mealPlan.IsTemplate;

            if (action == "Add sessions")
            {
                var skipped = ModelState.Keys;
                foreach (var key in skipped)
                    ModelState[key].Errors.Clear();

                if (mealPlan.MealPlanSessions == null) { mealPlan.MealPlanSessions = new List<MealPlanSession>(); }
                mealPlan.MealPlanSessions.Add(new MealPlanSession());

                return View("EditMealPlan", mealPlan);
            }
            else if (action == "Edit")
            {
                if (!ModelState.IsValid) return View(mealPlan);

                var actualMealPlan = await _planService.GetMealPlan(mealPlan.Id);

                if (actualMealPlan == null) return View("CustomError", "Error_NotFound");

                if (mealPlan.TrainerId != actualMealPlan.TrainerId) return View("CustomError", "Error_Unauthorized");

                if (mealPlan.MealPlanSessions == null) { mealPlan.MealPlanSessions = new List<MealPlanSession>(); }

                //remove from Plan the sessions which are not in the new sessions list
                foreach (MealPlanSession p in actualMealPlan.MealPlanSessions!)
                {
                    if (!mealPlan.MealPlanSessions!.Contains(p))
                    {
                        actualMealPlan.MealPlanSessions.Remove(p);
                    }
                }

                //add to Plan sessions the sessions that are in the new sessions list and aren't on Plan sessions list already
                foreach (MealPlanSession p in mealPlan.MealPlanSessions!)
                {
                    if (!actualMealPlan.MealPlanSessions!.Contains(p))
                    {
                        actualMealPlan.MealPlanSessions.Add(p);
                    }
                }

                actualMealPlan.Name = mealPlan.Name;
                actualMealPlan.Description = mealPlan.Description;

                if (!actualMealPlan.IsTemplate)
                {
                    actualMealPlan.StartDate = mealPlan.StartDate;
                    actualMealPlan.EndDate = mealPlan.EndDate;
                }

                await _planService.UpdateMealPlan(actualMealPlan);

                return RedirectToAction("MealTemplates");
            }

            return View("CustomError", "Error_NotFound");
        }

        public class ChooseTrainingTemplate
        {
            public IEnumerable<TrainingPlan> TrainingPlans { get; set; }
            public string AthleteId { get; set; }
        }

        public async Task<IActionResult> ChooseTrainingTemplates(string athleteId)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            return View(new ChooseTrainingTemplate { TrainingPlans = await _planService.GetTemplateTrainingPlans(userId), AthleteId = athleteId } );
        }

        public class ChooseMealTemplate
        {
            public IEnumerable<MealPlan> MealPlans { get; set; }
            public string AthleteId { get; set; }
        }

        public async Task<IActionResult> ChooseMealTemplates(string athleteId)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //Check if is Trainer of the active club
            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            return View(new ChooseMealTemplate { MealPlans = await _planService.GetTemplateMealPlans(userId), AthleteId = athleteId });
        }

    }

    
}
