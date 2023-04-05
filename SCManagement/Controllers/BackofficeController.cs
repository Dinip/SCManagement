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

        private string computeTimestampText(DateTime input, int? month = null)
        {
            if (month != null && month > 0 && month < 13)
            {
                return new DateTime(input.Year, input.Month, input.Day).ToString("dd MMMM yyyy", CultureInfo.CurrentCulture);
            }
            return new DateTime(input.Year, input.Month, 1).ToString("MMMM yyyy", CultureInfo.CurrentCulture);
        }

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

        public async Task<IActionResult> DelayedPayments()
        {
            return View(await _statisticsService.GetDelayedClubSubscriptions());
        }

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

        public async Task<IActionResult> PlansAdherenceData()
        {
            var stats = await _statisticsService.GetSystemPlansShortStatistics();
            return Json(new { data = stats });
        }

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

        public async Task<IActionResult> Clubs()
        {
            return View(await _statisticsService.GetClubsGeneralStats());
        }

        public async Task<IActionResult> Modalities()
        {
            ViewBag.Success = TempData["Success"];
            ViewBag.Cultures = new List<CultureInfo> { new("pt-PT"), new("en-US") };
            return View(await _clubService.GetModalities());
        }

        public async Task<IActionResult> CreateModality()
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

        public async Task<IActionResult> ManagePlans()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Error = TempData["Error"];
            return View(await _paymentService.GetClubSubscriptionPlans(true));
        }

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

        public async Task<IActionResult> CreatePlan()
        {
            ViewBag.Frequency = new SelectList(from SubscriptionFrequency sf in Enum.GetValues(typeof(SubscriptionFrequency)) select new { Id = (int)sf, Name = _stringLocalizer[sf.ToString()] }, "Id", "Name");

            return View();
        }

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

        public async Task<IActionResult> EditPlan(int id)
        {
            var plan = await _paymentService.GetClubSubscriptionPlan(id, true);
            if (plan == null) return View("CustomError", "Error_NotFound");

            bool anyUsing = await _paymentService.AnySubscriptionUsingPlan(plan.Id);

            ViewBag.Frequency = new SelectList(from SubscriptionFrequency sf in Enum.GetValues(typeof(SubscriptionFrequency)) select new { Id = (int)sf, Name = _stringLocalizer[sf.ToString()] }, "Id", "Name", plan.Frequency);
            ViewBag.Using = anyUsing;

            return View(new CustomPlanModel().ConvertFromProduct(plan));
        }

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
