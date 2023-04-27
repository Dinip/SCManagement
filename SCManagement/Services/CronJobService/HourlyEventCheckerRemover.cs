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


        /// <summary>
        /// Hourly event checker remover constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public HourlyEventCheckerRemover(
            IScheduleConfig<HourlyEventCheckerRemover> config,
            ILogger<HourlyEventCheckerRemover> logger,
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
            _logger.LogInformation("Hourly event checker remover starting...");
            return base.StartAsync(cancellationToken);
        }


        /// <summary>
        /// Actual job to be executed
        /// Gets all event payment enrolls that are still pending payment
        /// 4 hours after the creation and cancels them. Notifies the user
        /// that he was removed from the event.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Stops the job
        /// </summary>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Hourly event checker remover is stopping...");
            return base.StopAsync(cancellationToken);
        }
    }
}
