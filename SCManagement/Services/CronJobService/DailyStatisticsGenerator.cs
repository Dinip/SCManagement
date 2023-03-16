using SCManagement.Data;
using SCManagement.Services.StatisticsService;

namespace SCManagement.Services.CronJobService
{
    public class DailyStatisticsGenerator : CronJobService
    {
        private readonly ILogger<DailyStatisticsGenerator> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DailyStatisticsGenerator(
            IScheduleConfig<DailyStatisticsGenerator> config,
            ILogger<DailyStatisticsGenerator> logger,
            IServiceProvider serviceProvider
            ) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily statistics generator checker starting...");
            return base.StartAsync(cancellationToken);
        }

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

            return;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily statistics generator checker is stopping...");
            return base.StopAsync(cancellationToken);
        }
    }
}
