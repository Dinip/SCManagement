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

namespace SCManagement.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly BackgroundWorkerService _backgroundWorker;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly SharedResourceService _sharedResource;
        private readonly string _hostUrl;

        public NotificationService(
            BackgroundWorkerService backgroundWorker,
            IServiceProvider serviceProvider,
            IEmailNotificationService emailNotificationService,
            SharedResourceService sharedResource,
            IHttpContextAccessor httpContext
            )
        {
            _backgroundWorker = backgroundWorker;
            _serviceProvider = serviceProvider;
            _emailNotificationService = emailNotificationService;
            _sharedResource = sharedResource;
            _hostUrl = $"{httpContext.HttpContext.Request.Scheme}://{httpContext.HttpContext.Request.Host}";
        }

        public void NotifyQuotaUpdate(int clubId, ClubPaymentSettings newValues)
        {
            _backgroundWorker.Enqueue(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var partnersIds = await _context.UsersRoleClub.Where(u => u.ClubId == clubId && u.RoleId == 10).Select(u => u.UserId).ToListAsync();
                var clubName = await _context.Club.Where(c => c.Id == clubId).Select(c => c.Name).FirstAsync();

                var partners = await getUsersInfosToNotify(_context, partnersIds);

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
                        //_backgroundWorker.Enqueue(async () =>
                        //{
                        //    using var scope2 = _serviceProvider.CreateScope();
                        //    var _paymentService = scope2.ServiceProvider.GetService<IPaymentService>();

                        //    await _paymentService.CancelAutoSubscription(sub.Id);
                        //});
                    }

                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_FREQUENCY_", _sharedResource.Get(newValues.QuotaFrequency.ToString(), partner.Language) },
                        { "_PRICE_", $"{newValues.QuotaFee}€"},
                        { "_CLUB_", clubName },
                        { "_SUBSCRIPTION_", sub != null ? $"{_hostUrl}/Subscription?subId={sub.Id}" : $"{_hostUrl}/Subscription"},
                    };

                    _emailNotificationService.SendEmail(partner.Email, partner.Language, "ClubFees", values);
                }
                return;
            });
        }

        private async Task<ICollection<User>> getUsersInfosToNotify(ApplicationDbContext _context, ICollection<string> userIds)
        {
            //maybe also include notification settings to check if the
            //user has the specified notification enabled or not
            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
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
