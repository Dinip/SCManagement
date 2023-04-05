using SCManagement.Services.PaymentService.Models;
using SCManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using SCManagement.Services.NotificationService;

namespace SCManagement.Services.CronJobService
{
    public class DailySubscriptionSuspender : CronJobService
    {
        private readonly ILogger<DailySubscriptionSuspender> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DailySubscriptionSuspender(
            IScheduleConfig<DailySubscriptionSuspender> config,
            ILogger<DailySubscriptionSuspender> logger,
            IServiceProvider serviceProvider
            ) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily subscription suspender starting...");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} daily subscription suspender working...");
            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //get all subs that had a next time payment 3 days ago
            //and are still pending payment today
            var subs = await _context.Subscription
                .Include(p => p.Product)
                .Where(p => p.NextTime.Date == DateTime.Now.Date.AddDays(-3) && p.Status == SubscriptionStatus.Pending)
                .ToListAsync(cancellationToken);

            foreach (var sub in subs)
            {
                //suspend clubs
                if (sub.Product.ProductType == ProductType.ClubSubscription)
                {
                    //TODO
                    //notification service notify club admin
                    //that the club was suspended
                    var club = await _context.Club.FindAsync(sub.ClubId);
                    club!.Status = Models.ClubStatus.Suspended;
                    _context.Club.Update(club);
                }

                //remove partners
                if (sub.Product.ProductType == ProductType.ClubMembership)
                {
                    //TODO
                    //notification service notify user that
                    //was removed from club partners
                    var role = await _context.UsersRoleClub
                        .FirstOrDefaultAsync(f => f.UserId == sub.UserId && f.ClubId == sub.Product.ClubId && f.RoleId == 20);
                    
                    //find the respective role
                    if (role != null)
                    {
                        _context.UsersRoleClub.Remove(role);

                        //cancel sub
                        sub.Status = SubscriptionStatus.Canceled;
                        
                        //find associated payment (pending) and remove it
                        var pay = await _context.Payment
                            .FirstOrDefaultAsync(f => f.SubscriptionId == sub.Id && f.PaymentStatus == PaymentStatus.Pending);
                        if (pay != null)
                        {
                            pay.PaymentStatus = PaymentStatus.Canceled;
                            _context.Payment.Update(pay);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);

            //notification service notify each user that a
            //payment has been created and if is manual
            //renew that has X days to pay
            var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            _notificationService.NotifySubscriptionExpired(subs.Select(s => s.Id).ToList());

            return;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily subscription suspender is stopping...");
            return base.StopAsync(cancellationToken);
        }
    }
}
