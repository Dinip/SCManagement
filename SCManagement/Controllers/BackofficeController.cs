using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.NotificationService;
using SCManagement.Services.PaymentService;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.StatisticsService;
using SCManagement.Services.StatisticsService.Models;
using SCManagement.Services.TranslationService;
using SCManagement.Services.UserService;

namespace SCManagement.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BackofficeController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IStatisticsService _statisticsService;
        private readonly IPaymentService _paymentService;
        private readonly IClubService _clubService;
        private readonly ITranslationService _translationService;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Backoffice controller constructor, injects all the services needed
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userManager"></param>
        /// <param name="statisticsService"></param>
        /// <param name="paymentService"></param>
        /// <param name="clubService"></param>
        /// <param name="translationService"></param>
        /// <param name="stringLocalizer"></param>
        /// <param name="notificationService"></param>
        public BackofficeController(
            IUserService userService,
            UserManager<User> userManager,
            IStatisticsService statisticsService,
            IPaymentService paymentService,
            IClubService clubService,
            ITranslationService translationService,
            IStringLocalizer<SharedResource> stringLocalizer,
            INotificationService notificationService)
        {
            _userService = userService;
            _userManager = userManager;
            _statisticsService = statisticsService;
            _paymentService = paymentService;
            _clubService = clubService;
            _translationService = translationService;
            _stringLocalizer = stringLocalizer;
            _notificationService = notificationService;
        }

        private string getUserIdFromAuthedUser()
        {
            //reminder: this does not query the db, it just gets the id from the token (claims)
            return _userManager.GetUserId(User);
        }

        /// <summary>
        /// Helper method to compute the text to display for a given timestamp 
        /// If month is specified, it will display the day as well (dd MMMM yyyy)
        /// else it will display the month and year (MMMM yyyy)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private string computeTimestampText(DateTime input, int? month = null)
        {
            if (month != null && month > 0 && month < 13)
            {
                return new DateTime(input.Year, input.Month, input.Day).ToString("dd MMMM yyyy", CultureInfo.CurrentCulture);
            }
            return new DateTime(input.Year, input.Month, 1).ToString("MMMM yyyy", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Helper method to compute the last day of the month for all
        /// months of given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private List<DateTime> computeAllMonths(int? year = null)
        {
            year ??= DateTime.Now.Year;

            var months = new List<DateTime>();

            for (int i = 1; i <= 12; i++)
            {
                months.Add(new DateTime((int)year, i, DateTime.DaysInMonth((int)year, i)));
            }

            return months;
        }

        /// <summary>
        /// Gets short statistics for the backoffice dashboard
        /// and returns the view (with the circles)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var stats = new BackofficeStats
            {
                Clubs = await _statisticsService.GetActiveAndOtherClubsCount(),
                Income = await _statisticsService.GetMonthYearIncomeShort(),
                Codes = await _statisticsService.GetUsedAndCreatedCodes(),
                BestSeller = await _statisticsService.BestSellerPlan(),
                Payments = await _statisticsService.GetActiveAndDelayedClubSubscriptionsCount(),
            };

            return View(stats);
        }

        /// <summary>
        /// User access management view (to add or remove admin rights)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UserAccess()
        {
            var allUsers = await _userService.GetAllUsers();

            var orderedUsers = allUsers.OrderByDescending(u => u.IsAdmin).ThenBy(u => u.FullName).ToList();

            ViewBag.Error = TempData["Error"];
            ViewBag.Message = TempData["Message"];
            ViewBag.User = TempData["User"] ?? "";

            List<string> ignoredUsers = new List<string>();
            ignoredUsers.AddRange(orderedUsers
                .Where(u => u.Email == "admin@scmanagement.me" || u.Id == getUserIdFromAuthedUser())
                .Select(u => u.Id)
                .ToList());
            ViewBag.IgnoredUsers = ignoredUsers;

            return View(orderedUsers);
        }

        /// <summary>
        /// User access management post method (to add or remove admin rights)
        /// It receives the userId and newRole and checks if the change
        /// can be made (trying to admin an already admin user results in error
        /// and vice versa)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newRole"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserAccess(string userId, string newRole)
        {
            if (newRole != "Administrator" && newRole != "Regular")
            {
                TempData["Error"] = "Error_InvalidRole";
                return RedirectToAction(nameof(UserAccess));
            }

            var isAdmin = await _userService.UserIsAdmin(userId);
            TempData["User"] = (await _userService.GetUser(userId))?.FullName ?? "???";

            if (isAdmin && newRole == "Administrator")
            {
                TempData["Error"] = "Error_UserAlreadyAdmin";
                return RedirectToAction(nameof(UserAccess));
            }

            if (!isAdmin && newRole == "Regular")
            {
                TempData["Error"] = "Error_UserAlreadyNotAdmin";
                return RedirectToAction(nameof(UserAccess));
            }

            var success = await _userService.ChangeSystemUserRole(userId, newRole);
            if (success)
            {
                TempData["Message"] = $"Success_Is{(newRole == "Administrator" ? "Administrator" : "NotAdministrator")}";
            }
            else
            {
                TempData["Error"] = $"Error_UpdatingTo{(newRole == "Administrator" ? "Administrator" : "NotAdministrator")}";
            }

            return RedirectToAction(nameof(UserAccess));
        }

        /// <summary>
        /// Gets the lists of delayed payments to system products (club plans)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DelayedPayments()
        {
            return View(await _statisticsService.GetDelayedClubSubscriptions());
        }


        /// <summary>
        /// Manual payment (re)notification for delayed payments
        /// Sent do club admins
        /// </summary>
        /// <param name="subId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotifyMissingPayment(int subId)
        {
            var sub = await _paymentService.GetSubscription(subId);
            var payments = await _paymentService.GetPayments(sub.UserId);
            var missing = payments.FirstOrDefault(f => f.SubscriptionId == subId && f.PaymentStatus == PaymentStatus.Pending);
            if (missing != null)
            {
                _notificationService.NotifyPaymentLate(missing.Id);
            }

            return RedirectToAction(nameof(DelayedPayments));
        }

        /// <summary>
        /// Gets the income (revenue) data for system payments for a given year
        /// or by default, to the current year
        /// Used in charts and tables
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> IncomeData(int? year)
        {
            var stats = await _statisticsService.GetSystemPaymentStatistics(year);

            var stats2 = stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp),
                    ProductName = s.Product.Name,
                    s.Value
                })
                .ToList();

            var months = computeAllMonths(year);
            foreach (var month in months)
            {

                //check if stats2 has an entry with the same value as "month" in the TimeInDate field
                if (!stats2.Any(s => s.TimeInDate == month))
                {
                    stats2.Add(new { TimeInDate = month, TimeInText = computeTimestampText(month), ProductName = "None", Value = 0.0f });
                }
            }

            stats2.Sort((x, y) => x.TimeInDate.CompareTo(y.TimeInDate));

            return Json(new { data = stats2 });
        }


        public IActionResult Income()
        {
            return View();
        }

        public IActionResult CodesCreated()
        {
            return View();
        }

        public IActionResult Subscription()
        {
            return View();
        }

        public IActionResult Operations()
        {
            return View();
        }

        /// <summary>
        /// Gets statistics about the amount of active and canceled club plans subscriptions
        /// Used in charts and tables
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> PlansData(int? year)
        {
            var stats = await _statisticsService.GetSystemPlansStatistics(year);

            var stats2 = stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp),
                    ProductId = (int)s.ProductId,
                    ProductName = s.Product.Name,
                    s.Active,
                    s.Canceled
                })
                .ToList();

            var months = computeAllMonths(year);
            foreach (var month in months)
            {

                //check if stats2 has an entry with the same value as "month" in the TimeInDate field
                if (!stats2.Any(s => s.TimeInDate == month))
                {
                    stats2.Add(new { TimeInDate = month, TimeInText = computeTimestampText(month), ProductId = 0, ProductName = "None", Active = 0, Canceled = 0 });
                }
            }

            stats2.Sort((x, y) => x.TimeInDate.CompareTo(y.TimeInDate));


            return Json(new { data = stats2 });
        }

        /// <summary>
        /// Gets statistics about the adherence of club plans
        /// Used in charts and tables
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> PlansAdherenceData()
        {
            var stats = await _statisticsService.GetSystemPlansShortStatistics();
            return Json(new { data = stats });
        }

        /// <summary>
        /// Gets statistics about the number of atheletes in a give club over a given year
        /// Used in charts and tables
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> AthletesData(int clubId, int? year)
        {
            var stats = await _statisticsService.GetClubUserStatistics(clubId, 20, year);

            var stats2 =
                stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp),
                    s.Value
                })
                .ToList();

            var months = computeAllMonths(year);
            foreach (var month in months)
            {

                //check if stats2 has an entry with the same value as "month" in the TimeInDate field
                if (!stats2.Any(s => s.TimeInDate == month))
                {
                    stats2.Add(new { TimeInDate = month, TimeInText = computeTimestampText(month), Value = 0 });
                }
            }

            stats2.Sort((x, y) => x.TimeInDate.CompareTo(y.TimeInDate));

            return Json(new { data = stats2 });
        }

        /// <summary>
        /// Gets statistics about the number of partners in a give club over a given year
        /// Used in charts and tables
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> PartnersData(int clubId, int? year)
        {
            var stats = await _statisticsService.GetClubUserStatistics(clubId, 10, year);

            var stats2 =
                stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp),
                    s.Value
                })
                .ToList();

            var months = computeAllMonths(year);
            foreach (var month in months)
            {

                //check if stats2 has an entry with the same value as "month" in the TimeInDate field
                if (!stats2.Any(s => s.TimeInDate == month))
                {
                    stats2.Add(new { TimeInDate = month, TimeInText = computeTimestampText(month), Value = 0 });
                }
            }

            stats2.Sort((x, y) => x.TimeInDate.CompareTo(y.TimeInDate));

            return Json(new { data = stats2 });
        }

        /// <summary>
        /// Gets a list of all clubs in the system and their respetive
        /// state and number of users in the club
        /// Used in charts and tables
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Clubs()
        {
            return View(await _statisticsService.GetClubsGeneralStats());
        }

        /// <summary>
        /// Gets the list of all existing modalities
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Modalities()
        {
            ViewBag.Success = TempData["Success"];
            ViewBag.Cultures = new List<CultureInfo> { new("pt-PT"), new("en-US") };
            return View(await _clubService.GetModalities());
        }

        /// <summary>
        /// View to create a new modality
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateModality()
        {
            List<CultureInfo> cultures = new List<CultureInfo> { new("pt-PT"), new("en-US") };

            Modality modality = new Modality { ModalityTranslations = new List<ModalityTranslation>() };

            foreach (CultureInfo culture in cultures)
            {
                modality.ModalityTranslations.Add(new ModalityTranslation
                {
                    Value = "",
                    Language = culture.Name,
                    Atribute = "Name",
                });
            }

            return View(modality);
        }

        /// <summary>
        /// Create a new modality in the system
        /// </summary>
        /// <param name="modality"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateModality([Bind("Id", "ModalityTranslations")] Modality modality)
        {
            if (!ModelState.IsValid) return View(modality);

            int numberOfEmpty = 0;

            foreach (ModalityTranslation modalityTranslation in modality.ModalityTranslations)
            {
                if (modalityTranslation.Value == null || modalityTranslation.Value.Equals(""))
                {
                    numberOfEmpty++;
                }
            }

            if (numberOfEmpty == modality.ModalityTranslations.Count)
            {
                ViewBag.Error = _stringLocalizer["Error_RequiredModalName"];
                return View(modality);
            }

            await _translationService.Translate(modality.ModalityTranslations);

            await _clubService.CreateModality(modality);

            TempData["Success"] = "SuccessModality";
            return RedirectToAction(nameof(Modalities));
        }

        /// <summary>
        /// Gets the list of all existing club plans, including disabled
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManagePlans()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Error = TempData["Error"];
            return View(await _paymentService.GetClubSubscriptionPlans(true));
        }

        /// <summary>
        /// Toggles the status of a plan (enabled/disabled)
        /// Which removes or adds them to the public list of available plans
        /// Also notifies the subscriber users (which are using the plan
        /// in their club) if the plan was disabled (the current users can
        /// still pay them, until they change or cancel their subscription)
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePlan(int planId)
        {
            var plan = await _paymentService.GetProduct(planId);

            if (plan == null) return View("CustomError", "Error_NotFound");

            plan.Enabled = !plan.Enabled;
            var updated = await _paymentService.UpdateProduct(plan);
            if (plan.Enabled)
            {
                TempData["Message"] = _stringLocalizer["PlanUpdateStatusDisabled"].Value.Replace("_NAME_", plan.Name);
            }
            else
            {
                TempData["Message"] = _stringLocalizer["PlanUpdateStatusEnabled"].Value.Replace("_NAME_", plan.Name);
            }

            if (!plan.Enabled)
            {
                _notificationService.NotifyPlanDiscontinued(plan.Id);
            }

            return RedirectToAction(nameof(ManagePlans));
        }

        /// <summary>
        /// View to create a new plan
        /// </summary>
        /// <returns></returns>
        public IActionResult CreatePlan()
        {
            ViewBag.Frequency = new SelectList(from SubscriptionFrequency sf in Enum.GetValues(typeof(SubscriptionFrequency)) select new { Id = (int)sf, Name = _stringLocalizer[sf.ToString()] }, "Id", "Name");

            return View();
        }

        /// <summary>
        /// Create a new plan in the system
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlan([Bind("Name,Value,Frequency,Enabled,AthleteSlots")] CustomPlanModel plan)
        {
            ViewBag.Frequency = new SelectList(from SubscriptionFrequency sf in Enum.GetValues(typeof(SubscriptionFrequency)) select new { Id = (int)sf, Name = _stringLocalizer[sf.ToString()] }, "Id", "Name");

            if (!ModelState.IsValid) return View(plan);

            var created = await _paymentService.CreateProduct(plan.ConvertToProduct());
            TempData["Message"] = _stringLocalizer["PlanCreated"].Value.Replace("_NAME_", created.Name);

            return RedirectToAction(nameof(ManagePlans));
        }

        /// <summary>
        /// View to edit an existing plan (limited edition when already in use)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditPlan(int id)
        {
            var plan = await _paymentService.GetClubSubscriptionPlan(id, true);
            if (plan == null) return View("CustomError", "Error_NotFound");

            bool anyUsing = await _paymentService.AnySubscriptionUsingPlan(plan.Id);

            ViewBag.Frequency = new SelectList(from SubscriptionFrequency sf in Enum.GetValues(typeof(SubscriptionFrequency)) select new { Id = (int)sf, Name = _stringLocalizer[sf.ToString()] }, "Id", "Name", plan.Frequency);
            ViewBag.Using = anyUsing;

            return View(new CustomPlanModel().ConvertFromProduct(plan));
        }

        /// <summary>
        /// Edit an existing plan (limited edition when already in use)
        /// Also notifies the subscriber users (which are using the plan
        /// in their club) if the plan was disabled (the current users can
        /// still pay them, until they change or cancel their subscription)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="plan"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlan(int id, [Bind("Id,Name,Value,Frequency,Enabled,AthleteSlots")] CustomPlanModel plan)
        {
            if (plan == null) return View("CustomError", "Error_NotFound");
            ViewBag.Frequency = new SelectList(from SubscriptionFrequency sf in Enum.GetValues(typeof(SubscriptionFrequency)) select new { Id = (int)sf, Name = _stringLocalizer[sf.ToString()] }, "Id", "Name");
            bool anyUsing = await _paymentService.AnySubscriptionUsingPlan(plan.Id);
            ViewBag.Using = anyUsing;

            if (!ModelState.IsValid) return View(plan);

            var oldPlan = await _paymentService.GetClubSubscriptionPlan(id, true);
            if (oldPlan == null) return View("CustomError", "Error_NotFound");

            var oldEnabled = oldPlan.Enabled;

            //Limits the changes that can be made to a plan that is already in use
            //If is being "used" (any club with subscription) then only the name
            //and enabled status can be changed

            if (anyUsing)
            {
                oldPlan.Name = plan.Name;
                oldPlan.Enabled = plan.Enabled;
            }
            else
            {
                oldPlan.Name = plan.Name;
                oldPlan.Value = plan.Value;
                oldPlan.Frequency = plan.Frequency;
                oldPlan.Enabled = plan.Enabled;
                oldPlan.AthleteSlots = plan.AthleteSlots;
            }

            var updated = await _paymentService.UpdateProduct(oldPlan);
            TempData["Message"] = _stringLocalizer["PlanUpdated"].Value.Replace("_NAME_", updated.Name);

            if (oldEnabled && !updated.Enabled) //old was enabled and now is disabled
            {
                _notificationService.NotifyPlanDiscontinued(plan.Id);
            }

            return RedirectToAction(nameof(ManagePlans));
        }

        /// <summary>
        /// Delete an existing plan (only if not in use by any club)
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlan(int planId)
        {
            var plan = await _paymentService.GetProduct(planId);

            if (plan == null) return View("CustomError", "Error_NotFound");

            bool anyUsing = await _paymentService.AnySubscriptionUsingPlan(plan.Id);
            if (anyUsing) return View("CustomError", "Error_NotFound");

            await _paymentService.DeleteProduct(planId);
            TempData["Message"] = _stringLocalizer["PlanDeleteSuccess"].Value.Replace("_NAME_", plan.Name);

            return RedirectToAction(nameof(ManagePlans));
        }

        public class CustomPlanModel
        {
            public int Id { get; set; }

            [Display(Name = "Name")]
            [StringLength(50, ErrorMessage = "Error_Length", MinimumLength = 2)]
            public string Name { get; set; }

            [Display(Name = "Value")]
            [Range(0, float.MaxValue, ErrorMessage = "Error_MaxNumber")]
            public float Value { get; set; }

            [Display(Name = "Subscription Frequency")]
            public SubscriptionFrequency Frequency { get; set; }

            public bool Enabled { get; set; } = true;

            [Display(Name = "Athlete Slots")]
            [Range(1, int.MaxValue, ErrorMessage = "Error_MaxNumber")]
            public int AthleteSlots { get; set; }

            public Product ConvertToProduct()
            {
                return new Product
                {
                    Id = Id,
                    Name = Name,
                    Value = Value,
                    Frequency = Frequency,
                    Enabled = Enabled,
                    ProductType = ProductType.ClubSubscription,
                    AthleteSlots = AthleteSlots,
                    IsSubscription = true,
                };
            }

            public CustomPlanModel ConvertFromProduct(Product product)
            {
                return new CustomPlanModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Value = product.Value,
                    Frequency = product.Frequency.Value,
                    Enabled = product.Enabled,
                    AthleteSlots = product.AthleteSlots.Value,
                };
            }
        }
    }
}
