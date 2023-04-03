using System;
using System.Collections.Generic;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SCManagement.Data;
using SCManagement.Data.Migrations;
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
        private readonly SharedResourceService _sharedResource;
        private readonly string _hostUrl;

        public NotificationService(
            BackgroundWorkerService backgroundWorker,
            SharedResourceService sharedResource,
            IHttpContextAccessor httpContext
            )
        {
            _backgroundWorker = backgroundWorker;
            _sharedResource = sharedResource;
            _hostUrl = $"{httpContext.HttpContext.Request.Scheme}://{httpContext.HttpContext.Request.Host}";
        }

        private async Task<ICollection<User>> getUsersInfosToNotify(ApplicationDbContext _context, ICollection<string> userIds, NotificationType notificationType)
        {
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

        public void NotifyQuotaUpdate(int clubId, ClubPaymentSettings newValues)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var partnersIds = await _context.UsersRoleClub.Where(u => u.ClubId == clubId && u.RoleId == 10).Select(u => u.UserId).ToListAsync();
                var clubName = await _context.Club.Where(c => c.Id == clubId).Select(c => c.Name).FirstAsync();

                var partners = await getUsersInfosToNotify(_context, partnersIds, NotificationType.Club_Quota_Update);

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
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
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
                    Plan plan = plans.First(s => s.AthleteId == user.Id);

                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_PLANTYPE_", isMeal ? _sharedResource.Get("Meal Plan", user.Language) : _sharedResource.Get("Training Plan", user.Language)},
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_TRAINER_", trainerName },
                        { "_PLANURL_",  isMeal ? $"{_hostUrl}/Plans/MealDetails/{plan.Id}" : $"{_hostUrl}/Plans/TrainingDetails/{plan.Id}" },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "PlanCreated", values);
                }
            });
        }

        public void NotifyPlanEdit(Plan plan)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var trainerName = await _context.Users.Where(f => f.Id == plan.TrainerId).Select(f => f.FullName).FirstAsync();

                bool isMeal = plan.GetType() == typeof(MealPlan);
                NotificationType notificationType = isMeal ? NotificationType.MealPlan_Edited : NotificationType.TrainingPlan_Edited;

                var users = await getUsersInfosToNotify(_context, new List<string>() { plan.AthleteId }, notificationType);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_PLANTYPE_", isMeal ? _sharedResource.Get("Meal Plan", user.Language) : _sharedResource.Get("Training Plan", user.Language)},
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_TRAINER_", trainerName },
                        { "_PLANURL_",  isMeal ? $"{_hostUrl}/Plans/MealDetails/{plan.Id}" : $"{_hostUrl}/Plans/TrainingDetails/{plan.Id}" },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "PlanEdited", values);
                }
            });
        }

        public void NotifyPlanDeleted(Plan plan)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var trainerName = await _context.Users.Where(f => f.Id == plan.TrainerId).Select(f => f.FullName).FirstAsync();

                bool isMeal = plan.GetType() == typeof(MealPlan);
                NotificationType notificationType = isMeal ? NotificationType.MealPlan_Deleted : NotificationType.TrainingPlan_Deleted;

                var users = await getUsersInfosToNotify(_context, new List<string>() { plan.AthleteId }, notificationType);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_PLANTYPE_", isMeal ? _sharedResource.Get("Meal Plan", user.Language) : _sharedResource.Get("Training Plan", user.Language)},
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_TRAINER_", trainerName },
                        { "_PLANNAME_",  plan.Name }
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "PlanDeleted", values);
                }
            });
        }

        public void NotifyGoalCreate(Goal goal)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var trainerName = await _context.Users.Where(f => f.Id == goal.TrainerId).Select(f => f.FullName).FirstAsync();

                var users = await getUsersInfosToNotify(_context, new List<string>() { goal.AthleteId }, NotificationType.Goal_Assigned);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {   
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_TRAINER_", trainerName },
                        { "_GOALURL_", $"{_hostUrl}/Plans/GoalDetails/{goal.Id}" }
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "GoalCreated", values);
                }
            });
        }

        public void NotifyGoalEdited(Goal goal)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var trainerName = await _context.Users.Where(f => f.Id == goal.TrainerId).Select(f => f.FullName).FirstAsync();

                var users = await getUsersInfosToNotify(_context, new List<string>() { goal.AthleteId }, NotificationType.Goal_Edited);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_GOALNAME_", $"{goal.Name}"},
                        { "_TRAINER_", trainerName },
                        { "_GOALURL_", $"{_hostUrl}/Plans/GoalDetails/{goal.Id}" }
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "GoalEdited", values);
                }
            });
        }

        public void NotifyGoalDeleted(Goal goal)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var trainerName = await _context.Users.Where(f => f.Id == goal.TrainerId).Select(f => f.FullName).FirstAsync();

                var users = await getUsersInfosToNotify(_context, new List<string>() { goal.AthleteId }, NotificationType.Goal_Edited);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_GOALNAME_", $"{goal.Name}"},
                        { "_TRAINER_", trainerName },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "GoalEdited", values);
                }
            });
        }

        public void NotifyGoalCompleted(Goal goal)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var athelteName = await _context.Users.Where(f => f.Id == goal.AthleteId).Select(f => f.FullName).FirstAsync();

                var users = await getUsersInfosToNotify(_context, new List<string>() { goal.AthleteId }, NotificationType.Goal_Completed);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_GOALNAME_", $"{goal.Name}"},
                        { "_ATHLETE_", athelteName },
                        { "_GOALURL_", $"{_hostUrl}/Plans/GoalsList/{goal.AthleteId}" }
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "GoalCompleted", values);
                }
            });
        }

        public void NotifyTeamAdded(Team team, ICollection<string> userIds)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var trainerName = await _context.Users.Where(f => f.Id == team.TrainerId).Select(f => f.FullName).FirstAsync();

                var users = await getUsersInfosToNotify(_context, userIds, NotificationType.Team_Added);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_TEAMNAME_", $"{team.Name}"},
                        { "_TRAINER_", trainerName },
                        { "_TEAMURL_", $"{_hostUrl}/MyClub/TeamDetails/{team.Id}" },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "TeamAdded", values);
                }
            });
        }

        public void NotifyTeam_Removed(Team team, string userIds)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var trainerName = await _context.Users.Where(f => f.Id == team.TrainerId).Select(f => f.FullName).FirstAsync();

                var users = await getUsersInfosToNotify(_context, new List<string>() { userIds }, NotificationType.Team_Removed);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_TEAMNAME_", $"{team.Name}"},
                        { "_TRAINER_", trainerName },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "TeamRemoved", values);
                }
            });
        }

        public void NotifyEventCreate(Event eve)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var clubName = (await _context.Club.FirstOrDefaultAsync(f => f.Id == eve.ClubId)).Name;
                var usersToNotify = await _context.UsersRoleClub.Where(f => f.ClubId == eve.ClubId).Select(f => f.UserId).ToListAsync();

                var users = await getUsersInfosToNotify(_context, usersToNotify, NotificationType.Event_Created);

                foreach (var user in users)
                {
                    var eveName = eve.EventTranslations.Where(f => f.Language == user.Language && f.Atribute == "Name").Select(f => f.Value).First();
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_EVENTNAME_", $"{eveName}"},
                        { "_CLUBNAME_", clubName },
                        { "_EVENTURL_", $"{_hostUrl}/Events/Details/{eve.Id}" }
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "EventCreated", values);
                }
            });
        }

        public void NotifyEventEdit(Event eve)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var clubName = (await _context.Club.FirstOrDefaultAsync(f => f.Id == eve.ClubId)).Name;

                var users = await getUsersInfosToNotify(_context, eve.UsersEnrolled.Select(u => u.UserId).ToList(), NotificationType.Event_Edited);

                foreach (var user in users)
                {
                    var eveName = eve.EventTranslations.Where(f => f.Language == user.Language && f.Atribute == "Name").Select(f => f.Value).First();
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_EVENTNAME_", $"{eveName}"},
                        { "_CLUBNAME_", clubName },
                        { "_EVENTURL_", $"{_hostUrl}/Events/Details/{eve.Id}" }
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "EventEdited", values);
                }
            });
        }

        public void NotifyEventDeleted(Event eve)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var club = await _context.Club.FirstOrDefaultAsync(f => f.Id == eve.ClubId);
                var clubName = club.Name;

                var users = await getUsersInfosToNotify(_context, eve.UsersEnrolled.Select(u => u.UserId).ToList(), NotificationType.Event_Canceled);

                foreach (var user in users)
                {
                    var eveName = eve.EventTranslations.Where(f => f.Language == user.Language && f.Atribute == "Name").Select(f => f.Value).First();
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_EVENTNAME_", $"{eveName}"},
                        { "_CLUBNAME_", clubName },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "EventCanceled", values);
                }
            });
        }

        public void NotifyAthletesNumberAlmostFull(Product product)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var clubName = (await _context.Club.FirstOrDefaultAsync(f => f.Id == product.ClubId)).Name;

                //change this line for what u need
                List<string> strings = new List<string>();
                var users = await getUsersInfosToNotify(_context, strings, NotificationType.Athletes_Number_Almost_Full);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_CLUBNAME_", clubName },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "PaymentLate", values);
                }
            });
        }

        public void NotifyPaymentLate(Payment payment)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var clubId = (await _context.Product.FirstOrDefaultAsync(p => p.Id == payment.ProductId)).ClubId;
                var type = (await _context.Product.FirstOrDefaultAsync(p => p.Id == payment.ProductId)).ProductType;
                var clubName = (await _context.Club.FirstOrDefaultAsync(f => f.Id == clubId)).Name;

                //change this line for what u need
                List<string> strings = new List<string>();
                var users = await getUsersInfosToNotify(_context, strings, NotificationType.Payment_Late);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_CLUBNAME_", clubName },
                        { "_PRODUCTTYPE_", _sharedResource.Get(type.ToString(), user.Language) },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "PaymentLate", values);
                }
            });
        }

        public void NotifyPaymentReceived(Payment payment)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();

                var userName = (await _context.Users.FirstOrDefaultAsync(u => u.Id == payment.UserId)).FullName;

                //change this line for what u need
                List<string> strings = new List<string>();
                var users = await getUsersInfosToNotify(_context, strings, NotificationType.Payment_Received);

                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_PAYMENTUSERNAME_", userName },
                        { "_VALUE_", payment.Value.ToString() },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "PaymentReceived", values);
                }
            });
        }

        public void NotifySubscriptionCanceled(Subscription subscription)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();


                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_EVENTNAME_", $"{eveName}"},
                        { "_CLUBNAME_", clubName },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "SubscriptionCanceled", values);
                }
            });
        }

        public void NotifySubscriptionExpired(Subscription subscription)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();


                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_EVENTNAME_", $"{eveName}"},
                        { "_CLUBNAME_", clubName },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "SubscriptionExpired", values);
                }
            });
        }

        public void NotifySubscriptionExtended(Subscription subscription)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();


                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_EVENTNAME_", $"{eveName}"},
                        { "_CLUBNAME_", clubName },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "SubscriptionExtended", values);
                }
            });
        }

        public void NotifySubscriptionRenewed(Subscription subscription)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();


                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_EVENTNAME_", $"{eveName}"},
                        { "_CLUBNAME_", clubName },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "SubscriptionRenewed", values);
                }
            });
        }

        public void NotifySubscriptionStarted(Subscription subscription)
        {
            _backgroundWorker.Enqueue(async (_serviceProvider) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _backgroundHelperService = scope.ServiceProvider.GetRequiredService<IBackgroundHelperService>();


                foreach (var user in users)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "_USERNAME_", $"{user.FullName}"},
                        { "_EVENTNAME_", $"{eveName}"},
                        { "_CLUBNAME_", clubName },
                    };

                    _backgroundHelperService.SendEmail(user.Email, user.Language, "SubscriptionStarted", values);
                }
            });
        }
    }
}
