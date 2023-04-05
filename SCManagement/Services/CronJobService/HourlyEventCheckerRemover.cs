using SCManagement.Services.PaymentService.Models;
using SCManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using SCManagement.Services.NotificationService;

namespace SCManagement.Services.CronJobService
{
    public class HourlyEventCheckerRemover : CronJobService
    {
        private readonly ILogger<HourlyEventCheckerRemover> _logger;
        private readonly IServiceProvider _serviceProvider;

        public HourlyEventCheckerRemover(
            IScheduleConfig<HourlyEventCheckerRemover> config,
            ILogger<HourlyEventCheckerRemover> logger,
            IServiceProvider serviceProvider
            ) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hourly event checker remover starting...");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} hourly event checker remover working...");
            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //get all payments that are for events and were created 
            //4 hours ago and are still not payed
            var payments = await _context.Payment
                .Include(p => p.Product)
                .Where(p =>
                    p.Product.ProductType == ProductType.Event &&
                    p.CreatedAt <= DateTime.Now.AddHours(-4) &&
                    p.PaymentStatus == PaymentStatus.Pending
                )
                .ToListAsync(cancellationToken);

            //remove event enrollment and cancel payment
            foreach (var pay in payments)
            {
                pay.PaymentStatus = PaymentStatus.Canceled;
                _context.Payment.Update(pay);

                //notification service notify user that
                //was removed from event

                var enroll = await _context.EventEnroll
                    .FirstOrDefaultAsync(f => f.UserId == pay.UserId && f.EventId == pay.Product.OriginalId);
                if (enroll != null)
                {
                    _context.EventEnroll.Remove(enroll);
                    var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    _notificationService.NotifyEventLeft(enroll, true);
                }
            }
            await _context.SaveChangesAsync(cancellationToken);

            return;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hourly event checker remover is stopping...");
            return base.StopAsync(cancellationToken);
        }
    }
}
