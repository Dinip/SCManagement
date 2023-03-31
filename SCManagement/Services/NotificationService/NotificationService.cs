using System;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.BackgroundService;
using SCManagement.Services.PaymentService;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.PlansService.Models;
using static SCManagement.Models.Notification;

namespace SCManagement.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly BackgroundWorkerService _backgroundWorker;
        private readonly IServiceProvider _serviceProvider;
        private readonly SharedResourceService _sharedResource;
        private readonly string _hostUrl;

        public NotificationService(
            BackgroundWorkerService backgroundWorker,
            IServiceProvider serviceProvider,
            SharedResourceService sharedResource,
            IHttpContextAccessor httpContext
            )
        {
            _backgroundWorker = backgroundWorker;
            _serviceProvider = serviceProvider;
            _sharedResource = sharedResource;
            _hostUrl = $"{httpContext.HttpContext.Request.Scheme}://{httpContext.HttpContext.Request.Host}";
        }

        public void NotifyQuotaUpdate(int clubId, ClubPaymentSettings newValues)
        {
            _backgroundWorker.Enqueue(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var partnersIds = await _context.UsersRoleClub.Where(u => u.ClubId == clubId && u.RoleId == 10).Select(u => u.UserId).ToListAsync();
                var clubName = await _context.Club.Where(c => c.Id == clubId).Select(c => c.Name).FirstAsync();

                var partners = await getUsersInfosToNotify(_context, partnersIds,NotificationType.Club_Quota_Update);

                var product = await _context.Product
                    .Where(p =>
                        p.ClubId == clubId &&
                        p.ProductType == ProductType.ClubMembership
                    )
                    .Select(p => p.Id)
                    .FirstAsync();

                var subs = await _context.Subscription
                    .Where(s => s.ProductId == product)
                    .Select(s => new Subscription
                    {
                        Id = s.Id,
                        AutoRenew = s.AutoRenew,
                        UserId = s.UserId,
                    })
                    .ToListAsync();

                foreach (var partner in partners)
                {

                    var sub = subs.FirstOrDefault(s => s.UserId == partner.Id);

                    if (sub != null && sub.AutoRenew)
                    {
                        var pay = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                        await pay.CancelAutoSubscription(sub.Id);
                    }

                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_FREQUENCY_", _sharedResource.Get(newValues.QuotaFrequency.ToString(), partner.Language) },
                        { "_PRICE_", $"{newValues.QuotaFee}€"},
                        { "_CLUB_", clubName },
                        { "_SUBSCRIPTION_", sub != null ? $"{_hostUrl}/Subscription?subId={sub.Id}" : $"{_hostUrl}/Subscription"},
                    };

                    _backgroundHelperService.SendEmail(partner.Email, partner.Language, "ClubFees", values);
                }
                return;
            });
        }
       
        public void NotifyPlansCreate(IEnumerable<Plan> plans)
        {
            _backgroundWorker.Enqueue(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var aId = plans.Select(s => s.AthleteId.ToString()).ToList();

                var tId = plans.First().TrainerId;
                var trainerName = await _context.Users.Where(f => f.Id == tId).Select(f => f.FullName).FirstAsync();

                bool isMeal = plans.First().GetType() == typeof(MealPlan);
                NotificationType notificationType = isMeal ? NotificationType.MealPlan_Assigned : NotificationType.TrainingPlan_Assigned;

                var users = await getUsersInfosToNotify(_context, aId, notificationType);

                foreach (var user in users)
                {
                    if (user.Notifications.FirstOrDefault(n => n.Type == notificationType).IsEnabled)
                    {
                        Plan plan = plans.First(s => s.AthleteId == user.Id);

                        Dictionary<string, string> values = new Dictionary<string, string>
                        {
                            { "_PLANTYPE_", isMeal ? _sharedResource.Get("Meal Plan", user.Language) : _sharedResource.Get("Train", user.Language)},
                            { "_USERNAME_", $"{user.FullName}"},
                            { "_TRAINER_", trainerName },
                            { "_PLANURL_",  isMeal ? $"{_hostUrl}/Plans/MealDetails/{plan.Id}" : $"{_hostUrl}/Plans/TrainingDetails/{plan.Id}" },
                        };

                        _backgroundHelperService.SendEmail(user.Email, user.Language, "PlanCreated", values);
                    }
                    
                }
            });
        }


        private async Task<ICollection<User>> getUsersInfosToNotify(ApplicationDbContext _context, ICollection<string> userIds, NotificationType notificationType)
        {
            //maybe also include notification settings to check if the
            //user has the specified notification enabled or not
            return await _context.Users
                .Include(u => u.Notifications)
                .Where(u => userIds.Contains(u.Id) && u.Notifications.Any(n => n.Type == notificationType && n.IsEnabled == true))
                .Select(u => new User
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Language = u.Language
                })
                .ToListAsync();
        }
    }
}
