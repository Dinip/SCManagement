using SCManagement.Services.PaymentService.Models;
using SCManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;
using SCManagement.Models;

namespace SCManagement.Services.CronJobService
{
    public class DailySubscriptionChecker : CronJobService
    {
        private readonly ILogger<DailySubscriptionChecker> _logger;
        private readonly ApplicationDbContext _context;

        public DailySubscriptionChecker(
            IScheduleConfig<DailySubscriptionChecker> config,
            ILogger<DailySubscriptionChecker> logger,
            ApplicationDbContext context
            ) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _context = context;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily subscription checker starting...");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} daily subscription checker working...");

            //get all subs that renew today but aren't expected
            //to end today (canceled subs)
            var subs = await _context.Subscription
                .Where(p => p.NextTime.Date == DateTime.Now.Date && p.EndTime == null)
                .ToListAsync(cancellationToken);

            subs.ForEach(f => f.Status = SubscriptionStatus.Pending);

            //create payments
            var payments = subs.Select(s => new Payment
            {
                Value = s.Value,
                PaymentStatus = PaymentStatus.Pending,
                SubscriptionId = s.Id,
                UserId = s.UserId,
                ProductId = s.ProductId,
            }).ToList();

            //TODO
            //notification service notify each user that a
            //payment has been created and if is manual
            //renew that has X days to pay

            _context.Payment.AddRange(payments);
            _context.Subscription.UpdateRange(subs);
            await _context.SaveChangesAsync(cancellationToken);

            return;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily subscription checker is stopping...");
            return base.StopAsync(cancellationToken);
        }
    }
}
