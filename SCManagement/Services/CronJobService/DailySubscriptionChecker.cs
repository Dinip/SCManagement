using SCManagement.Services.PaymentService.Models;
using SCManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using SCManagement.Services.NotificationService;

namespace SCManagement.Services.CronJobService
{
    public class DailySubscriptionChecker : CronJobService
    {
        private readonly ILogger<DailySubscriptionChecker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DailySubscriptionChecker(
            IScheduleConfig<DailySubscriptionChecker> config,
            ILogger<DailySubscriptionChecker> logger,
            IServiceProvider serviceProvider
            ) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily subscription checker starting...");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} daily subscription checker working...");
            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //get all subs that renew today but aren't expected
            //to end today (canceled subs)
            var subs = await _context.Subscription
                .Where(p => p.NextTime.Date == DateTime.Now.Date && p.EndTime == null)
                .ToListAsync(cancellationToken);

            //update all subscriptions to pending if they are not free (0€)
            subs.ForEach(f => { if (f.Value > 0) f.Status = SubscriptionStatus.Pending; });

            //create payments
            //if value is 0 (free) consider it paid
            var payments = subs.Select(s => new Payment
            {
                Value = s.Value,
                PaymentStatus = s.Value > 0 ? (s.AutoRenew ? PaymentStatus.Processing : PaymentStatus.Pending) : PaymentStatus.Paid,
                SubscriptionId = s.Id,
                UserId = s.UserId,
                ProductId = s.ProductId,
            }).ToList();

            _context.Payment.AddRange(payments);
            _context.Subscription.UpdateRange(subs);
            await _context.SaveChangesAsync(cancellationToken);

            //notification service notify each user that a
            //payment has been created and if is manual
            //renew that has X days to pay
            var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            _notificationService.NotifySubscriptionRenewTime(subs.Select(s=>s.Id).ToList());

            return;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily subscription checker is stopping...");
            return base.StopAsync(cancellationToken);
        }
    }
}
