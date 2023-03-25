using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Models;
using SCManagement.Services.PaymentService;
using SCManagement.Services.StatisticsService;
using SCManagement.Services.StatisticsService.Models;
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

        public BackofficeController(IUserService userService, UserManager<User> userManager, IStatisticsService statisticsService, IPaymentService paymentService)
        {
            _userService = userService;
            _userManager = userManager;
            _statisticsService = statisticsService;
            _paymentService = paymentService;
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
                BestSeller = await _statisticsService.BestSellerPlan()
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
        [AutoValidateAntiforgeryToken]
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
            return View(await _paymentService.GetDelayedClubSubscriptions());
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> NotifyMissingPayment(int subId)
        {
            //NOTIFICATION CONTROLLER

            return RedirectToAction(nameof(DelayedPayments));
        }


        public async Task<IActionResult> Income(int? year)
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


        public async Task<IActionResult> Plans(int? year)
        {
            var stats = await _statisticsService.GetSystemPlansStatistics(year);

            var stats2 = stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp),
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
                    stats2.Add(new { TimeInDate = month, TimeInText = computeTimestampText(month), ProductName = "None", Active = 0, Canceled = 0 });
                }
            }

            stats2.Sort((x, y) => x.TimeInDate.CompareTo(y.TimeInDate));


            return Json(new { data = stats2 });
        }
    }
}
