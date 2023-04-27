using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Services;
using SCManagement.Services.StatisticsService;

namespace SCManagement.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly ApplicationContextService _applicationContextService;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        /// <summary>
        /// Statistics controller constructor, injects all the services needed
        /// </summary>
        /// <param name="statisticsService"></param>
        /// <param name="applicationContextService"></param>
        /// <param name="stringLocalizer"></param>
        public StatisticsController(IStatisticsService statisticsService,
            ApplicationContextService applicationContextService,
            IStringLocalizer<SharedResource> stringLocalizer)
        {
            _statisticsService = statisticsService;
            _applicationContextService = applicationContextService;
            _stringLocalizer = stringLocalizer;
        }


        /// <summary>
        /// Index page for club statistics (with circles, charts and tables)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();

            return View(await _statisticsService.GetCurrentClubUsersStatistics(_applicationContextService.UserRole.ClubId));
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
        /// Gets statistics about club received payments (revenue)
        /// for a given year
        /// Returns all months (12) even if there are no payments or
        /// not already passed that month, so that the chart and table
        /// can show all year's months.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> Payments(int? year)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubPaymentStatistics(_applicationContextService.UserRole.ClubId, year);

            var stats2 =
                stats
                .GroupBy(s => s.Timestamp)
                .Select(s => new
                {
                    TimeInDate = s.Key,
                    TimeInText = computeTimestampText(s.Key),
                    Events = s.Where(f => f.ProductType == Services.PaymentService.Models.ProductType.Event).Sum(x => x.Value),
                    ClubFee = s.Where(f => f.ProductType == Services.PaymentService.Models.ProductType.ClubMembership).Sum(x => x.Value),
                })
                .ToList();

            var months = computeAllMonths(year);
            foreach (var month in months)
            {

                //check if stats2 has an entry with the same value as "month" in the TimeInDate field
                if (!stats2.Any(s => s.TimeInDate == month))
                {
                    stats2.Add(new { TimeInDate = month, TimeInText = computeTimestampText(month), Events = 0.0f, ClubFee = 0.0f });
                }
            }

            stats2.Sort((x, y) => x.TimeInDate.CompareTo(y.TimeInDate));

            return Json(new { data = stats2 });
        }


        /// <summary>
        /// Gets detailed statistics about club received payments (revenue)
        /// Agregates by product and month for a given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> PaymentsDetailed(int? year)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubPaymentDetailsStatistics(_applicationContextService.UserRole.ClubId, year);

            var stats2 =
                stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp),
                    ProductId = (int)s.ProductId,
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
                    stats2.Add(new { TimeInDate = month, TimeInText = computeTimestampText(month), ProductId = 0, ProductName = "None", Value = 0.0f });
                }
            }

            stats2.Sort((x, y) => x.TimeInDate.CompareTo(y.TimeInDate));

            return Json(new { data = stats2 });
        }

        /// <summary>
        /// Gets statistics about the number of athletes in the club
        /// by each month of a given year
        /// Returns all months (12) even if there are no payments or
        /// not already passed that month, so that the chart and table
        /// can show all year's months.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> Athletes(int? year)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubUserStatistics(_applicationContextService.UserRole.ClubId, 20, year);

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
        /// Gets statistics about the number of partners in the club
        /// by each month of a given year
        /// Returns all months (12) even if there are no payments or
        /// not already passed that month, so that the chart and table
        /// can show all year's months.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> Partners(int? year)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubUserStatistics(_applicationContextService.UserRole.ClubId, 10, year);

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
        /// Gets statistics about the number of athletes associated
        /// to each modality (that a club has) by each month of a given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IActionResult> Modalities(int? year)
        {
            if (_applicationContextService.UserRole.RoleId < 40) return NotFound();
            var stats = await _statisticsService.GetClubModalityStatistics(_applicationContextService.UserRole.ClubId, year);
            
            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            
            var stats2 =
                stats
                .Select(s => new
                {
                    TimeInDate = s.Timestamp,
                    TimeInText = computeTimestampText(s.Timestamp),
                    s.ModalityId,
                    ModalityName = s.Modality.ModalityTranslations.Where(m => m.Language == cultureInfo).First().Value,
                    s.Value
                })
                .ToList();

            var months = computeAllMonths(year);
            foreach (var month in months)
            {

                //check if stats2 has an entry with the same value as "month" in the TimeInDate field
                if (!stats2.Any(s => s.TimeInDate == month))
                {
                    stats2.Add(new { TimeInDate = month, TimeInText = computeTimestampText(month), ModalityId = 0, ModalityName = "None", Value = 0 });
                }
            }

            stats2.Sort((x, y) => x.TimeInDate.CompareTo(y.TimeInDate));

            return Json(new { data = stats2 });
        }
    }
}
