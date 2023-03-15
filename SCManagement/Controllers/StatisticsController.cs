using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SCManagement.Models;
using SCManagement.Services;
using SCManagement.Services.StatisticsService;
using SCManagement.Services.StatisticsService.Models;

namespace SCManagement.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly ApplicationContextService _applicationContextService;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;


        public StatisticsController(IStatisticsService statisticsService,
            ApplicationContextService applicationContextService,
            IStringLocalizer<SharedResource> stringLocalizer)
        {
            _statisticsService = statisticsService;
            _applicationContextService = applicationContextService;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<IActionResult> Index()
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();

            return View(await _statisticsService.GetCurrentClubUsersStatistics(_applicationContextService.UserRole.ClubId));
        }

        private string computeTimestampText(DateTime input, int? month)
        {
            if (month != null && month > 0 && month < 13)
            {
                return new DateTime(input.Year, input.Month, input.Day).ToString("dd MMMM yyyy", CultureInfo.CurrentCulture);
            }
            return new DateTime(input.Year, input.Month, 1).ToString("MMMM yyyy", CultureInfo.CurrentCulture);
        }

        public async Task<IActionResult> Payments(int? year, int? month)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubPaymentStatistics(_applicationContextService.UserRole.ClubId, year, month);

            var stats2 =
                stats
                .GroupBy(s => s.Timestamp)
                .Select(s => new
                {
                    TimeInDate = s.Key,
                    TimeInText = computeTimestampText(s.Key, month),
                    Events = s.Where(f => f.ProductType == Services.PaymentService.Models.ProductType.Event).Sum(x => x.Value),
                    ClubFee = s.Where(f => f.ProductType == Services.PaymentService.Models.ProductType.ClubMembership).Sum(x => x.Value),
                })
                .ToList();

            return Json(new { data = stats2 });
        }

        public async Task<IActionResult> PaymentsDetailed(int? year, int? month)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubPaymentDetailsStatistics(_applicationContextService.UserRole.ClubId, year, month);

            var stats2 =
                stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp, month),
                    s.ProductId,
                    ProductName = s.Product.Name,
                    s.Value
                })
                .ToList();

            return Json(new { data = stats2 });
        }

        public async Task<IActionResult> Athletes(int? year, int? month)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubUserStatistics(_applicationContextService.UserRole.ClubId, 20, year, month);

            var stats2 =
                stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp, month),
                    s.Value
                })
                .ToList();

            return Json(new { data = stats2 });
        }

        public async Task<IActionResult> Partners(int? year, int? month)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubUserStatistics(_applicationContextService.UserRole.ClubId, 10, year, month);

            var stats2 =
                stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp, month),
                    s.Value
                })
                .ToList();

            return Json(new { data = stats2 });
        }

        public async Task<IActionResult> Modalities(int? year, int? month)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubModalityStatistics(_applicationContextService.UserRole.ClubId, year, month);

            var stats2 =
                stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp, month),
                    s.ModalityId,
                    ModalityName = _stringLocalizer[s.Modality.Name].Value,
                    s.Value
                })
                .ToList();

            return Json(new { data = stats2 });
        }
    }
}
