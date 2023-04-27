using SCManagement.Data;
using SCManagement.Services.StatisticsService;

namespace SCManagement.Services.CronJobService
{
    public class DailyStatisticsGenerator : CronJobService
    {
        private readonly ILogger<DailyStatisticsGenerator> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Daily statistics generator constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public DailyStatisticsGenerator(
            IScheduleConfig<DailyStatisticsGenerator> config,
            ILogger<DailyStatisticsGenerator> logger,
            IServiceProvider serviceProvider
            ) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        /// <summary>
        /// Starts the job
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily statistics generator checker starting...");
            return base.StartAsync(cancellationToken);
        }


        /// <summary>
        /// Actual job to be executed
        /// Gets all clubs that are not suspended or deleted already
        /// and creates statistics for them
        /// Also generates system statistics (club plans and payments)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} statistics generator checker working...");
            using var scope = _serviceProvider.CreateScope();
            var statisticsService = scope.ServiceProvider.GetRequiredService<IStatisticsService>();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var clubIds = _context.Club
                .Where(c => c.Status != Models.ClubStatus.Suspended && c.Status != Models.ClubStatus.Deleted)
                .Select(c => c.Id).ToList();

            foreach (var clubId in clubIds)
            {
                await statisticsService.CreateClubUserStatistics(clubId);
                await statisticsService.CreateClubPaymentStatistics(clubId);
                await statisticsService.CreateClubModalityStatistics(clubId);
            }

            await statisticsService.CreateSystemPaymentStatistics();
            await statisticsService.CreateSystemPlansStatistics();

            return;
        }


        /// <summary>
        /// Stops the job
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily statistics generator checker is stopping...");
            return base.StopAsync(cancellationToken);
        }
    }
}
