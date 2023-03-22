using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SCManagement.Data.Migrations;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PlansService;
using SCManagement.Services.PlansService.Models;
using SCManagement.Services.TeamService;
using SCManagement.Services.UserService;

namespace SCManagement.Controllers
{
    [Authorize]
    public class PlansController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;
        private readonly IPlanService _planService;
        private readonly ITeamService _teamService;

        public PlansController(
            UserManager<User> userManager,
            IClubService clubService,
            IUserService userService,
            IPlanService planService,
            ITeamService teamService)
        {
            _userManager = userManager;
            _clubService = clubService;
            _userService = userService;
            _planService = planService;
            _teamService = teamService;
        }

        private string getUserIdFromAuthedUser()
        {
            //reminder: this does not query the db, it just gets the id from the token (claims)
            return _userManager.GetUserId(User);
        }

        public class TemplatesLists
        {
            public List<TrainingPlan> TrainingPlans { get; set; }
            public List<MealPlan> MealPlans { get; set; }
        }

        public async Task<IActionResult> Templates()
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.IsTemplate = true;

            var trains = await _planService.GetTemplateTrainingPlans(userId);

            if (trains == null) return View("CustomError", "Error_NotFound");

            var meals = await _planService.GetTemplateMealPlans(userId);

            if (meals == null) return View("CustomError", "Error_NotFound");

            var templates = new TemplatesLists
            {
                TrainingPlans = (List<TrainingPlan>)trains,
                MealPlans = (List<MealPlan>)meals
            };

            return View(templates);
        }

        public async Task<IActionResult> TrainingTemplates()
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.IsTemplate = true;

            var trains = await _planService.GetTemplateTrainingPlans(userId);

            if (trains == null) return View("CustomError", "Error_NotFound");

            return View("TrainingPlans", trains);
        }

        public async Task<IActionResult> AthleteTrainingPlans(string id)
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.IsTemplate = false;
            ViewBag.AthleteName = (await _userService.GetUser(id)).FullName;

            var trains = await _planService.GetTrainingPlans(userId, id);

            if (trains == null) return View("CustomError", "Error_NotFound");

            return View("TrainingPlans", trains);
        }

        public async Task<IActionResult> MealTemplates()
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.IsTemplate = true;

            var meals = await _planService.GetTemplateMealPlans(userId);

            if (meals == null) return View("CustomError", "Error_NotFound");

            return View("MealPlans", meals);
        }

        public async Task<IActionResult> AthleteMealPlans(string id)
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.IsTemplate = false;
            ViewBag.AthleteName = (await _userService.GetUser(id)).FullName;

            var meals = await _planService.GetMealPlans(userId, id);

            if (meals == null) return View("CustomError", "Error_NotFound");

            return View("MealPlans", meals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrainingPlan(int? id)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var plan = await _planService.GetTrainingPlan((int)id);

            if (plan == null) return View("CustomError", "Error_NotFound");

            if (plan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            await _planService.DeleteTrainingPlan(plan);

            if (!plan.IsTemplate) return RedirectToAction(nameof(AthleteTrainingPlans), new { id = plan.AthleteId });

            return RedirectToAction(nameof(Templates));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteMealPlan(int? id)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var plan = await _planService.GetMealPlan((int)id);

            if (plan == null) return View("CustomError", "Error_NotFound");

            if (plan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            await _planService.DeleteMealPlan(plan);

            if (!plan.IsTemplate) return RedirectToAction(nameof(AthleteTrainingPlans), new { id = plan.AthleteId });

            return RedirectToAction(nameof(Templates));
        }

        public async Task<IActionResult> CreateTrainingPlan(string? athleteId, string? isTemplate, int? id, int? teamId)
        {
            string userId = getUserIdFromAuthedUser();

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

            ViewBag.IsTeam = false;
            if (teamId != null)
            {
                ViewBag.IsTeam = true;
                ViewBag.TeamId = teamId;
            }

            return View("CreateTrainingPlan", trains);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainingPlan([Bind("Name, Description, StartDate, EndDate, TrainerId, AthleteId, ModalityId, IsTemplate, TrainingPlanSessions")] TrainingPlan trainingPlan, string action, string? apply, int? teamId)
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(role.ClubId), "Id", "Name");
            ViewBag.IsTemplate = trainingPlan.IsTemplate;
            ViewBag.Apply = Convert.ToBoolean(apply);
            ViewBag.IsTeam = teamId != null;
            ViewBag.TeamId = teamId;

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

                if (teamId != null)
                {
                    var team = await _teamService.GetTeam((int)teamId);

                    if (team == null) return View("CustomError", "Error_NotFound");
                    if (team.Trainer.Id != userId) return View("CustomError", "Error_Unauthorized");

                    foreach (User athlete in team.Athletes)
                    {
                        var trains = new TrainingPlan()
                        {
                            Name = trainingPlan.Name,
                            Description = trainingPlan.Description,
                            TrainerId = trainingPlan.TrainerId,
                            AthleteId = athlete.Id,
                            IsTemplate = false,
                            StartDate = trainingPlan.StartDate,
                            EndDate = trainingPlan.EndDate,
                            ModalityId = trainingPlan.ModalityId,
                            TrainingPlanSessions = trainingPlan.TrainingPlanSessions.Select(s => new TrainingPlanSession()
                            {
                                ExerciseName = s.ExerciseName,
                                ExerciseDescription = s.ExerciseDescription,
                                Duration = s.Duration,
                                Repetitions = s.Repetitions,
                            }).ToList()
                        };

                        await _planService.CreateTrainingPlan(trains);
                    }
                }
                else
                {
                    await _planService.CreateTrainingPlan(trainingPlan);
                }

                if (trainingPlan.IsTemplate) return RedirectToAction("Templates");

                return RedirectToAction("TrainingZone", "MyClub");
            }

            return View("CustomError", "Error_NotFound");
        }

        public async Task<IActionResult> CreateMealPlan(string? athleteId, string? isTemplate, int? id, int? teamId)
        {
            string userId = getUserIdFromAuthedUser();

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

            ViewBag.IsTeam = false;
            if (teamId != null)
            {
                ViewBag.IsTeam = true;
                ViewBag.TeamId = teamId;
            }


            return View("CreateMealPlan", meals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMealPlan([Bind("Name, Description, StartDate, EndDate, TrainerId, AthleteId, IsTemplate, MealPlanSessions")] MealPlan mealPlan, string action, string? apply, int? teamId)
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.IsTemplate = mealPlan.IsTemplate;
            ViewBag.Apply = Convert.ToBoolean(apply);
            ViewBag.IsTeam = teamId != null;
            ViewBag.TeamId = teamId;

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

                if (teamId != null)
                {
                    var team = await _teamService.GetTeam((int)teamId);

                    if (team == null) return View("CustomError", "Error_NotFound");
                    if (team.Trainer.Id != userId) return View("CustomError", "Error_Unauthorized");

                    foreach (User athlete in team.Athletes)
                    {
                        var meal = new MealPlan()
                        {
                            Name = mealPlan.Name,
                            Description = mealPlan.Description,
                            TrainerId = mealPlan.TrainerId,
                            AthleteId = athlete.Id,
                            IsTemplate = false,
                            StartDate = mealPlan.StartDate,
                            EndDate = mealPlan.EndDate,
                            MealPlanSessions = mealPlan.MealPlanSessions.Select(s => new MealPlanSession()
                            {
                                MealName = s.MealName,
                                MealDescription = s.MealDescription,
                                Time = s.Time,
                            }).ToList()
                        };

                        await _planService.CreateMealPlan(meal);
                    }
                }
                else
                {
                    foreach (MealPlanSession p in mealPlan.MealPlanSessions!)
                    {
                        p.Time = TimeSpan.Parse(p.Time.ToString());
                    }
                    
                    await _planService.CreateMealPlan(mealPlan);
                }

                if (mealPlan.IsTemplate) return RedirectToAction("Templates");

                return RedirectToAction("TrainingZone", "MyClub");
            }

            return View("CustomError", "Error_NotFound");
        }

        public async Task<IActionResult> EditTrainingPlan(int? id)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var trainingPlan = await _planService.GetTrainingPlan((int)id);

            if (trainingPlan == null) return View("CustomError", "Error_NotFound");

            if (trainingPlan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(role.ClubId), "Id", "Name");
            ViewBag.IsTemplate = trainingPlan.IsTemplate;

            return View(trainingPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainingPlan([Bind("Id, Name, Description, StartDate, EndDate, TrainerId, AthleteId, ModalityId, IsTemplate, TrainingPlanSessions")] TrainingPlan trainingPlan, string action)
        {
            string userId = getUserIdFromAuthedUser();

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

                if (!actualTrainingPlan.IsTemplate) return RedirectToAction(nameof(AthleteTrainingPlans), new { id = actualTrainingPlan.AthleteId });

                return RedirectToAction("Templates");
            }

            return View("CustomError", "Error_NotFound");
        }

        public async Task<IActionResult> EditMealPlan(int? id)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var mealPlan = await _planService.GetMealPlan((int)id);

            if (mealPlan == null) return View("CustomError", "Error_NotFound");

            if (mealPlan.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            ViewBag.IsTemplate = mealPlan.IsTemplate;

            return View(mealPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMealPlan([Bind("Id, Name, Description, StartDate, EndDate, TrainerId, AthleteId, IsTemplate, MealPlanSessions")] MealPlan mealPlan, string action)
        {
            string userId = getUserIdFromAuthedUser();
            
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

                if (!actualMealPlan.IsTemplate) return RedirectToAction(nameof(AthleteMealPlans), new { id = actualMealPlan.AthleteId });

                return RedirectToAction("Templates");
            }

            return View("CustomError", "Error_NotFound");
        }

        public class ChooseTrainingTemplate
        {
            public IEnumerable<TrainingPlan> TrainingPlans { get; set; }
            public string AthleteId { get; set; }
        }

        public async Task<IActionResult> ChooseTrainingTemplates(string id)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var template = await _planService.GetTemplateTrainingPlans(userId);

            if (template == null) return View("CustomError", "Error_NotFound");


            return View(new ChooseTrainingTemplate { TrainingPlans = template, AthleteId = id });
        }

        public class ChooseMealTemplate
        {
            public IEnumerable<MealPlan> MealPlans { get; set; }
            public string AthleteId { get; set; }
        }

        public async Task<IActionResult> ChooseMealTemplates(string id)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var template = await _planService.GetTemplateMealPlans(userId);

            if (template == null) return View("CustomError", "Error_NotFound");

            return View(new ChooseMealTemplate { MealPlans = template, AthleteId = id });
        }

        public async Task<IActionResult> TrainingDetails(int id)
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role) && !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            var actualTrainingPlan = await _planService.GetTrainingPlan(id);

            if (actualTrainingPlan == null) return View("CustomError", "Error_NotFound");

            if (actualTrainingPlan.TrainerId != userId && actualTrainingPlan.AthleteId != userId) return View("CustomError", "Error_Unauthorized");

            return View(actualTrainingPlan);
        }

        public async Task<IActionResult> MealDetails(int id)
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role) && !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            var actualMealPlan = await _planService.GetMealPlan(id);

            if (actualMealPlan == null) return View("CustomError", "Error_NotFound");

            if (actualMealPlan.TrainerId != userId && actualMealPlan.AthleteId != userId) return View("CustomError", "Error_Unauthorized");

            return View(actualMealPlan);
        }

        public async Task<IActionResult> CreateGoal(string id)
        {
            if(id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var goal = new Goal
            {
                Name = "",
                Description = "",
                AthleteId = id,
                TrainerId = userId,
            };

            return View(goal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGoal([Bind("TrainerId, AthleteId, Name, Description, StartDate, EndDate")] Goal goal)
        {
            if (!ModelState.IsValid) return View(goal);

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            await _planService.CreateGoal(goal);

            //if u need u can change this 
            return RedirectToAction("TrainingZone", "MyClub");
        }

        public async Task<IActionResult> EditGoal(int id)
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var goal = await _planService.GetGoal(id);

            if (goal == null) return View("CustomError", "Error_NotFound");

            if (goal.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            return View(goal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGoal([Bind("Id,TrainerId, AthleteId, Name, Description, StartDate, EndDate")] Goal goal)
        {
            if (!ModelState.IsValid) return View(goal);

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            if (goal.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            await _planService.UpdateGoal(goal);

            //if u need u can change this 
            return RedirectToAction("GoalsList", new { id = goal.AthleteId });
        }
        
        public async Task<IActionResult> GoalDetails(int id)
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role) && !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            var goal = await _planService.GetGoal(id);
             
            if (goal == null) return View("CustomError", "Error_NotFound");

            if (goal.TrainerId != userId && goal.AthleteId != userId) return View("CustomError", "Error_Unauthorized");
            
            return View(goal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            var goal = await _planService.GetGoal(id);

            if (goal == null) return View("CustomError", "Error_NotFound");

            if (goal.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            await _planService.DeleteGoal(goal);

            //if u need u can change this 
            return RedirectToAction("GoalsList", new { id = goal.AthleteId });
        }

        public async Task<IActionResult> GoalsList(string id)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            if (!_clubService.IsClubTrainer(role) && !_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            IEnumerable<Goal> goals;
            
            if (_clubService.IsClubAthlete(role)) 
            {
                goals = await _planService.GetGoals(id);
            }
            else 
            {
                goals = await _planService.GetGoals(userId,id);
            } 

            if (goals == null) return View("CustomError", "Error_NotFound");

            return View(goals);
        }
    }
    
}
