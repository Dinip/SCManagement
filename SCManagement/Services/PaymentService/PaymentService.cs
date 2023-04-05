using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.NotificationService;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.StatisticsService.Models;

namespace SCManagement.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        public PaymentService(ApplicationDbContext context, IConfiguration configuration, INotificationService notificationService)
        {
            _context = context;
            _configuration = configuration;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Creates an http client with the correct api key/secret, based
        /// on the clubId passed. If clubId is null, it assumes that should
        /// use the system credentials (eg. subscribe to a club plan)
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns>HttpClient</returns>
        private async Task<HttpClient> createHttpClient(int? clubId = null)
        {
            var client = new HttpClient();
            if (clubId == null)
            {
                client.DefaultRequestHeaders.Add("AccountId", _configuration["Easypay-SystemAccountId"]);
                client.DefaultRequestHeaders.Add("ApiKey", _configuration["Easypay-SystemAccountKey"]);
            }
            else
            {
                var club = await _context.ClubPaymentSettings.FirstAsync(c => c.ClubPaymentSettingsId == clubId);
                client.DefaultRequestHeaders.Add("AccountId", club.AccountId);
                client.DefaultRequestHeaders.Add("ApiKey", club.AccountKey);
            }

            return client;
        }

        /// <summary>
        /// Get all payments from a user, ordered by date
        /// from the most recent to the oldest
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Payment>> GetPayments(string userId)
        {
            return await _context.Payment
                .Include(p => p.Product)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get a specifid product
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Product info or null</returns>
        public async Task<Product?> GetProduct(int id)
        {
            return await _context.Product
                .Include(p => p.Club)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Creates a new subscription on easypay service and returns the data
        /// from easypay
        /// </summary>
        /// <param name="product"></param>
        /// <param name="user"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private async Task<EasypayResponse?> SubscriptionApiRequest(Product product, User user, DateTime startTime)
        {
            var client = await createHttpClient(product.ClubId);
            var result = await client.PostAsJsonAsync("https://api.test.easypay.pt/2.0/subscription", new
            {
                currency = "EUR",
                value = product.Value,
                frequency = Subscription.ConvertFrequency(product.Frequency),
                method = "cc",
                start_time = startTime.ToString("yyyy-MM-dd HH:mm"),
                expiration_time = DateTime.Now.AddYears(2).ToString("yyyy-MM-dd HH:mm"),
                capture = new
                {
                    descriptive = product.Name
                },
                customer = new
                {
                    name = user!.FullName,
                    email = user!.Email,
                    phone = user.PhoneNumber ?? ""
                }
            });

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Error in sub");
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return null;
            }

            return await result.Content.ReadFromJsonAsync<EasypayResponse>();
        }

        /// <summary>
        /// Creates a single payment on easypay service and returns the data
        /// </summary>
        /// <param name="paymentInput"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<EasypayResponse?> SinglePaymentApiRequest(Payment paymentInput, User user)
        {
            var client = await createHttpClient(paymentInput.Product.ClubId);
            var result = await client.PostAsJsonAsync("https://api.test.easypay.pt/2.0/single", new
            {
                type = "sale",
                currency = "EUR",
                value = paymentInput.Value,
                method = Payment.ConvertMethod(paymentInput.PaymentMethod.Value),
                capture = new
                {
                    descriptive = paymentInput.Product.Name
                },
                customer = new
                {
                    name = user!.FullName,
                    email = user!.Email,
                    phone = paymentInput.PhoneNumber ?? user.PhoneNumber ?? ""
                }
            });

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Error in single payment");
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return null;
            }

            return await result.Content.ReadFromJsonAsync<EasypayResponse>();
        }

        /// <summary>
        /// Gets single payment data from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Payment?> GetPayment(int id)
        {
            return await _context.Payment
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Gets all subscriptions from a user, ordered by next time date
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Subscription>> GetSubscriptions(string userId)
        {
            return await _context.Subscription
                .Include(s => s.Product)
                .Include(s => s.Club)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.NextTime)
                .ToListAsync();
        }

        /// <summary>
        /// Gets single subscription data from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Subscription?> GetSubscription(int id)
        {
            return await _context.Subscription
                .Include(s => s.Product)
                .Include(s => s.Club)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Returns the current subscription of a user membership to a club or null
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<Subscription?> GetMembershipSubscription(string userId, int clubId)
        {
            return await _context.Subscription
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.ClubId == clubId && s.Product.ProductType == ProductType.ClubMembership);
        }

        /// <summary>
        /// Gets data from a single payment on easypay service and returns the data from them
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        private async Task<EasypayResponse?> singlePaymentIdApiRequest(string id, int? clubId)
        {
            var client = await createHttpClient(clubId);
            var result = await client.GetAsync($"https://api.test.easypay.pt/2.0/single/{id}");

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Error in singlePaymentIdApiRequest");
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return null;
            }

            return await result.Content.ReadFromJsonAsync<EasypayResponse>();
        }

        /// <summary>
        /// Gets data from a subscription on easypay service and returns the data from them
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        private async Task<EasypayResponse?> subscriptionPaymentIdApiRequest(string id, int? clubId)
        {
            var client = await createHttpClient(clubId);
            var result = await client.GetAsync($"https://api.test.easypay.pt/2.0/subscription/{id}");

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Error in subscriptionPaymentIdApiRequest");
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return null;
            }

            return await result.Content.ReadFromJsonAsync<EasypayResponse>();
        }

        /// <summary>
        /// Deletes a subscription on easypay service and returns true if success
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        private async Task<bool> deleteSubscriptionId(string id, int? clubId)
        {
            var client = await createHttpClient(clubId);
            var result = await client.DeleteAsync($"https://api.test.easypay.pt/2.0/subscription/{id}");

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Error in deleteSubscriptionId");
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Helper function to create a json object with credit card
        /// info, to be stored on the associated transaction
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string? buildCardInfo(EasypayResponse info)
        {
            if (info.method.type == "CC" &&
                !string.IsNullOrEmpty(info.method.last_four) &&
                !string.IsNullOrEmpty(info.method.expiration_date) &&
                !string.IsNullOrEmpty(info.method.card_type))
            {
                return JsonConvert.SerializeObject(new CardInfo
                {
                    LastFourDigits = info.method.last_four,
                    ExpirationDate = info.method.expiration_date,
                    Type = info.method.card_type
                });
            }
            return null;
        }

        /// <summary>
        /// Handles logic from a single payment data from easypay webhook
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WebhookHandleSinglePayment(PaymentWebhookGeneric data)
        {
            var payment = await _context.Payment.Include(p => p.Product).FirstOrDefaultAsync(p => p.PaymentKey == data.id);
            if (payment == null) return;

            var info = await singlePaymentIdApiRequest(data.id, payment.Product.ClubId);
            if (info == null) return;

            if (info.payment_status != null) payment.PaymentStatus = Payment.ConvertStatus(info.payment_status);
            if (payment.PaymentStatus == PaymentStatus.Paid) payment.PayedAt = DateTime.Now;

            payment.CardInfoData = buildCardInfo(info);

            var clubIdInClubSub = 0;

            if (payment.SubscriptionId != null)
            {
                var subscription = await _context.Subscription.FindAsync(payment.SubscriptionId);
                if (subscription != null)
                {
                    subscription.Status = SubscriptionStatus.Active;
                    subscription.NextTime = DateTime.Now.Add(Subscription.AddTime(subscription.Frequency)).Date.Add(new TimeSpan(1, 0, 0));
                    clubIdInClubSub = (int)subscription.ClubId;
                    _context.Subscription.Update(subscription);
                }
            }

            if (payment.Product.ProductType == ProductType.Event && payment.PaymentStatus == PaymentStatus.Paid)
            {
                await updateEventEnroll((int)payment.Product.OriginalId, payment.UserId, EnrollPaymentStatus.Valid);
            }

            if (payment.Product.ProductType == ProductType.ClubSubscription && payment.PaymentStatus == PaymentStatus.Paid)
            {
                await updateClubSubStatus(clubIdInClubSub, ClubStatus.Active);
            }

            if (payment.Product.ProductType == ProductType.ClubMembership && payment.PaymentStatus == PaymentStatus.Paid)
            {
                await updateClubMembershipStatus((int)payment.Product.ClubId, payment.UserId, UserRoleStatus.Active);
            }

            _context.Payment.Update(payment);
            await _context.SaveChangesAsync();
            _notificationService.NotifyPaymentReceived(payment.Id);
        }

        /// <summary>
        /// Updates a user event enrolment when the payment is completed
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private async Task updateEventEnroll(int eventId, string userId, EnrollPaymentStatus status)
        {
            var enroll = await _context.EventEnroll.FirstAsync(e => e.UserId == userId && e.EventId == eventId);
            enroll.EnrollStatus = status;
            _context.EventEnroll.Update(enroll);
            _notificationService.NotifyEventJoined(enroll, false);
        }

        /// <summary>
        /// Updates club status when the payment is completed
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="status"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task updateClubSubStatus(int clubId, ClubStatus status, DateTime? endDate = null)
        {
            var club = await _context.Club.FirstAsync(e => e.Id == clubId);
            club.Status = status;
            if (endDate != null)
            {
                club.EndDate = endDate;
            }
            _context.Club.Update(club);
        }

        /// <summary>
        /// Updates club membership status when the payment is completed
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private async Task updateClubMembershipStatus(int clubId, string userId, UserRoleStatus status)
        {
            if (status == UserRoleStatus.Canceled)
            {
                var role = await _context.UsersRoleClub.FirstAsync(e => e.ClubId == clubId && e.UserId == userId && e.RoleId == 10);
                _context.UsersRoleClub.Remove(role);
            }
            else
            {
                var role = await _context.UsersRoleClub.FirstAsync(e => e.ClubId == clubId && e.UserId == userId && e.RoleId == 10);
                role.Status = status;
                _context.UsersRoleClub.Update(role);
            }
        }

        /// <summary>
        /// Handles logic from a subscription creation data from easypay webhook
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WebhookHandleSubscriptionCreate(PaymentWebhookGeneric data)
        {
            var subscription = await _context.Subscription.Include(p => p.Product).FirstOrDefaultAsync(s => s.SubscriptionKey == data.id);
            if (subscription == null) return;

            var info = await subscriptionPaymentIdApiRequest(data.id, subscription.Product.ClubId);
            if (info == null) return;

            subscription.CardInfoData = buildCardInfo(info);
            subscription.Status = Subscription.ConvertStatus(info.method.status);

            _context.Subscription.Update(subscription);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Handles logic from payment recieved associated with a subscription from easypay webhook
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WebhookHandleSubscriptionPayment(PaymentWebhookGeneric data)
        {
            var subscription = await _context.Subscription.Include(p => p.Product).FirstOrDefaultAsync(s => s.SubscriptionKey == data.id);
            if (subscription == null) return;

            var info = await subscriptionPaymentIdApiRequest(data.id, subscription.Product.ClubId);
            if (info == null) return;

            var payment = await _context.Payment.Where(p => p.SubscriptionId == subscription.Id && p.PaymentStatus != PaymentStatus.Paid).FirstOrDefaultAsync();
            if (payment == null)
            {
                payment = new Payment
                {
                    ProductId = subscription.ProductId,
                    Value = subscription.Value,
                    PaymentMethod = PaymentMethod.CreditCard,
                    PaymentStatus = PaymentStatus.Paid,
                    UserId = subscription.UserId,
                    PaymentKey = data.id,
                    SubscriptionId = subscription.Id,
                    CardInfoData = buildCardInfo(info),
                    PayedAt = DateTime.Now
                };
                _context.Payment.Update(payment);
                await _context.SaveChangesAsync();
            }
            else
            {
                payment.CardInfoData = buildCardInfo(info);
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.PaymentMethod = PaymentMethod.CreditCard;
                payment.PayedAt = DateTime.Now;
                _context.Payment.Update(payment);
            }

            subscription.NextTime = DateTime.Now.Add(Subscription.AddTime(subscription.Frequency)).Date.Add(new TimeSpan(1, 0, 0));
            subscription.CardInfoData = buildCardInfo(info);
            subscription.Status = Subscription.ConvertStatus(info.method.status);

            if (subscription.Product.ProductType == ProductType.ClubSubscription)
            {
                await updateClubSubStatus((int)subscription.ClubId, ClubStatus.Active);
            }

            if (subscription.Product.ProductType == ProductType.ClubMembership)
            {
                await updateClubMembershipStatus((int)subscription.Product.ClubId, subscription.UserId, UserRoleStatus.Active);
            }

            _context.Subscription.Update(subscription);
            await _context.SaveChangesAsync();
            _notificationService.NotifySubscriptionRenewed(subscription.Id);
            _notificationService.NotifyPaymentReceived(payment.Id);
        }

        /// <summary>
        /// Gets all available club subscription plans (enabled)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetClubSubscriptionPlans(bool? includeDisabled = false)
        {
            var query = _context.Product.Where(p => p.ProductType == ProductType.ClubSubscription);

            if (includeDisabled != true)
            {
                query = query.Where(p => p.Enabled == true);
            }

            return await query.OrderBy(p => p.AthleteSlots).ToListAsync();
        }

        /// <summary>
        /// Creates a subscription to a plan for a club
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public async Task<Subscription> SubscribeClubToPlan(int clubId, string userId, int planId)
        {
            var product = await GetProduct(planId);
            if (product == null || product.ProductType != ProductType.ClubSubscription)
            {
                product = (await GetClubSubscriptionPlans()).OrderBy(f => f.Value).First();
            }

            DateTime now = DateTime.Now;

            var sub = new Subscription
            {
                StartTime = now,
                NextTime = now,
                Value = product.Value,
                Status = product.Value > 0 ? SubscriptionStatus.Waiting : SubscriptionStatus.Active,
                ProductId = product.Id,
                UserId = userId,
                AutoRenew = false,
                Frequency = product.Frequency.Value,
                ClubId = clubId
            };

            _context.Subscription.Add(sub);
            await _context.SaveChangesAsync();

            var pay = new Payment
            {
                ProductId = product.Id,
                Value = product.Value,
                CreatedAt = now,
                PaymentStatus = product.Value > 0 ? PaymentStatus.Pending : PaymentStatus.Paid,
                UserId = userId,
                SubscriptionId = sub.Id,
                PaymentKey = Guid.NewGuid().ToString(),
            };

            _context.Payment.Add(pay);
            await _context.SaveChangesAsync();

            _notificationService.NotifySubscriptionStarted(sub.Id);

            return sub;
        }

        /// <summary>
        /// Updates subscription to auto renew
        /// </summary>
        /// <param name="subId"></param>
        /// <returns></returns>
        public async Task<Subscription?> SetSubscriptionToAuto(int subId)
        {
            //verificar a que clube pertence o produto (no caso de evento/mensalidade) e ir buscar a api key do clube

            var subscription = await _context.Subscription.Include(s => s.Product).Include(s => s.User).FirstOrDefaultAsync(s => s.Id == subId);
            if (subscription == null) return null;

            var startTime = DateTime.Now.AddMinutes(1);
            if (subscription.NextTime.Date != DateTime.Now.Date)
            {
                startTime = subscription.NextTime;
            }

            var content = await SubscriptionApiRequest(subscription.Product!, subscription.User!, startTime);
            if (content == null) return null;

            subscription.AutoRenew = true;
            subscription.SubscriptionKey = content.id;
            subscription.Status = SubscriptionStatus.Waiting;
            subscription.ConfigUrl = content.method.url;

            _context.Subscription.Update(subscription);

            var payment = await _context.Payment.FirstOrDefaultAsync(p => p.SubscriptionId == subscription.Id && p.PaymentStatus != PaymentStatus.Paid);
            if (payment != null)
            {
                payment.PaymentStatus = PaymentStatus.Processing;
                payment.PaymentMethod = PaymentMethod.CreditCard;
                _context.Payment.Update(payment);
            }
            await _context.SaveChangesAsync();
            return subscription;
        }

        /// <summary>
        /// Cancels subscription auto renew
        /// </summary>
        /// <param name="subId"></param>
        /// <returns></returns>
        public async Task<Subscription?> CancelAutoSubscription(int subId)
        {
            var subscription = await _context.Subscription.Include(s => s.Product).Include(s => s.User).FirstOrDefaultAsync(s => s.Id == subId);
            if (subscription == null || string.IsNullOrEmpty(subscription.SubscriptionKey)) return null;

            var success = await deleteSubscriptionId(subscription.SubscriptionKey!, subscription.Product.ClubId);

            if (!success) return null;

            subscription.AutoRenew = false;
            subscription.SubscriptionKey = null;
            subscription.ConfigUrl = null;
            subscription.Status = SubscriptionStatus.Active;
            subscription.CardInfoData = null;

            //when removing a auto sub, check next payment is in the same day
            //and if so, create a payment object
            if (DateTime.Now.Date == subscription.NextTime.Date)
            {
                subscription.Status = SubscriptionStatus.Pending;
                var oldPayment = await _context.Payment.FirstOrDefaultAsync(p => p.SubscriptionId == subscription.Id && p.PaymentStatus != PaymentStatus.Paid);
                if (oldPayment == null)
                {
                    var payment = new Payment
                    {
                        ProductId = subscription.Product.Id,
                        Value = subscription.Product.Value,
                        PaymentMethod = null,
                        PaymentStatus = PaymentStatus.Pending,
                        UserId = subscription.UserId,
                        PaymentKey = Guid.NewGuid().ToString(),
                        SubscriptionId = subscription.Id,
                    };
                    _context.Payment.Add(payment);
                }
                else
                {
                    oldPayment.PaymentMethod = null;
                    oldPayment.PaymentStatus = PaymentStatus.Pending;
                    _context.Payment.Update(oldPayment);
                }
            }

            _context.Subscription.Update(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        /// <summary>
        /// Cancels subscription
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task CancelSubscription(int id)
        {
            var subscription = await _context.Subscription.Include(p => p.Product).FirstOrDefaultAsync(s => s.Id == id);
            if (subscription == null) return;


            //cancel easypay subscription
            if (!string.IsNullOrEmpty(subscription.SubscriptionKey))
            {
                var success = await deleteSubscriptionId(subscription.SubscriptionKey!, subscription.Product.ClubId);
                subscription.AutoRenew = false;
                subscription.SubscriptionKey = null;
                subscription.CardInfoData = null;
            }

            //cancel pending payments
            var payments = await _context.Payment.Where(p => p.SubscriptionId == id && p.PaymentStatus == PaymentStatus.Pending).ToListAsync();
            if (payments.Any())
            {
                payments.ForEach(p => p.PaymentStatus = PaymentStatus.Canceled);
                _context.Payment.UpdateRange(payments);
            }

            subscription.Status = subscription.NextTime.Date <= DateTime.Now.Date ? SubscriptionStatus.Canceled : SubscriptionStatus.Pending_Cancel;
            subscription.EndTime = subscription.NextTime.Date <= DateTime.Now.Date ? DateTime.Now : subscription.NextTime;


            if (subscription.Product.ProductType == ProductType.ClubSubscription)
            {
                await updateClubSubStatus((int)subscription.ClubId, subscription.Status == SubscriptionStatus.Canceled ? ClubStatus.Suspended : ClubStatus.Active);
            }

            if (subscription.Product.ProductType == ProductType.ClubMembership)
            {
                await updateClubMembershipStatus((int)subscription.Product.ClubId, subscription.UserId, subscription.Status == SubscriptionStatus.Canceled ? UserRoleStatus.Canceled : UserRoleStatus.Pending_Cancel);
            }

            _context.Subscription.Update(subscription);
            await _context.SaveChangesAsync();
            _notificationService.NotifySubscriptionCanceled(subscription);
            return;
        }

        /// <summary>
        /// Update subscription value in easypay
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newValue"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        private async Task<bool> updateSubscription(string id, float newValue, int? clubId)
        {
            var client = await createHttpClient(clubId);
            var result = await client.PatchAsJsonAsync($"https://api.test.easypay.pt/2.0/subscription/{id}", new
            {
                value = newValue
            });

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Error in updateSubscription");
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Upgrades a subscription from a plan to a new one
        /// </summary>
        /// <param name="subId"></param>
        /// <param name="newPlanId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpgradeClubPlan(int subId, int newPlanId)
        {
            var subscription = await GetSubscription(subId);
            if (subscription == null)
            {
                throw new Exception("Error_NotFound");
            }

            var product = await GetProduct(newPlanId);
            if (product == null || product.ProductType != ProductType.ClubSubscription || !product.Enabled)
            {
                throw new Exception("Error_InvalidProduct");
            }

            var athletes = await _context.UsersRoleClub.Where(u => u.ClubId == subscription.ClubId && u.RoleId == 20).CountAsync();
            if (athletes > product.AthleteSlots)
            {
                throw new Exception("Não pode alterar para um plano com menos atletas que o número de atletas do clube");
            }


            if (subscription.AutoRenew)
            {
                var res = await updateSubscription(subscription.SubscriptionKey, product.Value, product.ClubId);
                if (!res)
                {
                    throw new Exception("Error updating subscription value");
                }
            }

            subscription.ProductId = newPlanId;
            subscription.Value = product.Value;
            subscription.Frequency = product.Frequency.Value;

            //find a pending payment in the last 3 days and update it to the new value and product
            var payment = await _context.Payment.FirstOrDefaultAsync(p =>
                p.SubscriptionId == subscription.Id && p.PaymentStatus == PaymentStatus.Pending && p.CreatedAt.Date >= DateTime.Now.Date.AddDays(-3)
            );

            if (payment != null)
            {
                payment.Value = product.Value;
                payment.ProductId = newPlanId;
                _context.Payment.Update(payment);
            }

            _context.Subscription.Update(subscription);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a product based on the event data
        /// </summary>
        /// <param name="myEvent"></param>
        /// <returns></returns>
        public async Task CreateProductEvent(Event myEvent)
        {
            _context.Product.Add(new Product
            {
                ClubId = myEvent.ClubId,
                IsSubscription = false,
                Enabled = true,
                Name = myEvent.EventTranslations.Where(e => e.Language == "en-US" && e.Atribute == "Name").Select(e => e.Value).FirstOrDefault(),
                OriginalId = myEvent.Id,
                ProductType = ProductType.Event,
                Value = myEvent.Fee
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates a product based on the event data
        /// </summary>
        /// <param name="myEvent"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public async Task UpdateProductEvent(Event myEvent, bool delete = false)
        {
            var oldProduct = await _context.Product.Where(p => p.OriginalId == myEvent.Id && p.ClubId == myEvent.ClubId && p.ProductType == ProductType.Event).FirstOrDefaultAsync();

            if (delete)
            {
                if (oldProduct == null) return;
                oldProduct.Enabled = false;
                _context.Product.Update(oldProduct);
                await _context.SaveChangesAsync();
                return;
            }

            var name = myEvent.EventTranslations.Where(e => e.Language == "en-US" && e.Atribute == "Name").Select(e => e.Value).FirstOrDefault();

            if (oldProduct == null)
            {
                _context.Product.Add(new Product
                {
                    ClubId = myEvent.ClubId,
                    IsSubscription = false,
                    Enabled = true,
                    Name = name,
                    OriginalId = myEvent.Id,
                    ProductType = ProductType.Event,
                    Value = myEvent.Fee
                });
                await _context.SaveChangesAsync();
                return;
            }

            if (oldProduct != null && myEvent.Fee == 0)
            {
                oldProduct.Enabled = false;
            }

            if (oldProduct != null && myEvent.Fee > 0)
            {
                oldProduct.Enabled = true;
            }

            oldProduct.Name = name;
            oldProduct.Value = myEvent.Fee;
            _context.Update(oldProduct);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates an event payment for the user enroll
        /// </summary>
        /// <param name="enroll"></param>
        /// <returns></returns>
        public async Task<Payment?> CreateEventPayment(EventEnroll enroll)
        {
            var product = await _context.Product.Where(p => p.ProductType == ProductType.Event && p.OriginalId == enroll.EventId && p.Enabled && p.Value != 0).FirstOrDefaultAsync();
            if (product == null) return null;

            var pay = new Payment
            {
                ProductId = product.Id,
                UserId = enroll.UserId,
                Value = product.Value,
                PaymentKey = Guid.NewGuid().ToString(),
            };

            _context.Payment.Add(pay);
            await _context.SaveChangesAsync();

            return pay;
        }

        /// <summary>
        /// Handles a single payment request
        /// </summary>
        /// <param name="paymentInput"></param>
        /// <returns></returns>
        public async Task<Payment?> PaySinglePayment(PayPayment paymentInput)
        {
            var payment = await GetPayment(paymentInput.Id);
            if (payment == null) return null;

            payment.PaymentMethod = paymentInput.PaymentMethod;
            payment.PhoneNumber = paymentInput.PhoneNumber;

            var user = await _context.Users.FirstAsync(u => u.Id == payment.UserId);

            var content = await SinglePaymentApiRequest(payment, user);
            if (content == null) return null;

            payment.PaymentKey = content.id;
            payment.MbEntity = content.method.entity;
            payment.MbReference = content.method.reference;
            payment.Url = content.method.url;
            payment.PaymentStatus = PaymentStatus.Processing;

            _context.Payment.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        /// <summary>
        /// Creates/updates club membership product based on club payment settings
        /// </summary>
        /// <param name="clubPaymentSettings"></param>
        /// <returns></returns>
        public async Task UpdateProductClubMembership(ClubPaymentSettings clubPaymentSettings)
        {
            var oldProduct = await _context.Product.Where(p => p.ClubId == clubPaymentSettings.ClubPaymentSettingsId && p.ProductType == ProductType.ClubMembership).FirstOrDefaultAsync();
            var clubName = await _context.Club.Where(c => c.Id == clubPaymentSettings.ClubPaymentSettingsId).Select(c => c.Name).FirstAsync();

            if (oldProduct == null)
            {
                _context.Product.Add(new Product
                {
                    ClubId = clubPaymentSettings.ClubPaymentSettingsId,
                    IsSubscription = true,
                    Enabled = true,
                    ProductType = ProductType.ClubMembership,
                    Value = clubPaymentSettings.QuotaFee,
                    Frequency = clubPaymentSettings.QuotaFrequency,
                    Name = $"Club Quota ({clubName})"
                });
                await _context.SaveChangesAsync();
                return;
            }

            oldProduct.Name = $"Club Quota ({clubName})";
            oldProduct.Value = clubPaymentSettings.QuotaFee;
            oldProduct.Frequency = clubPaymentSettings.QuotaFrequency;
            _context.Update(oldProduct);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Requests a config from easypay (used to test account credentials)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<EasypayConfigResponse?> easypayConfigRequest(string id, string key)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("AccountId", id);
            client.DefaultRequestHeaders.Add("ApiKey", key);

            var result = await client.GetAsync("https://api.test.easypay.pt/2.0/config");

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Error in EasypayConfigRequest");
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return null;
            }

            return await result.Content.ReadFromJsonAsync<EasypayConfigResponse>();
        }

        /// <summary>
        /// Tests if the account credentials are valid (easypay)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task TestAccount(string id, string key)
        {
            var result = await easypayConfigRequest(id, key);

            if (result == null)
            {
                throw new Exception("Error_InvalidCredentials");
            }

            if (string.IsNullOrEmpty(result.generic) || !result.generic.ToLower().EndsWith("scmanagement.me/api/payment/webhookgeneric"))
            {
                throw new Exception("Error_CallbackUrl");
            }
        }


        /// <summary>
        /// Creates a club membership subscription for a given user in a given club
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Subscription> CreateMembershipSubscription(string userId, int clubId)
        {
            var product = await _context.Product.FirstAsync(p => p.ProductType == ProductType.ClubMembership && p.ClubId == clubId);

            DateTime now = DateTime.Now;

            var sub = new Subscription
            {
                StartTime = now,
                NextTime = now,
                Value = product.Value,
                Status = product.Value > 0 ? SubscriptionStatus.Waiting : SubscriptionStatus.Active,
                ProductId = product.Id,
                UserId = userId,
                AutoRenew = false,
                Frequency = product.Frequency.Value,
            };

            _context.Subscription.Add(sub);
            await _context.SaveChangesAsync();

            var pay = new Payment
            {
                ProductId = product.Id,
                Value = product.Value,
                CreatedAt = now,
                PaymentStatus = product.Value > 0 ? PaymentStatus.Pending : PaymentStatus.Paid,
                UserId = userId,
                SubscriptionId = sub.Id,
                PaymentKey = Guid.NewGuid().ToString(),
            };

            _context.Payment.Add(pay);
            await _context.SaveChangesAsync();
            _notificationService.NotifySubscriptionStarted(sub.Id);

            return sub;
        }

        /// <summary>
        /// Cancel a payment for a given event enrolment
        /// </summary>
        /// <param name="enroll"></param>
        /// <returns></returns>
        public async Task CancelEventPayment(EventEnroll enroll)
        {
            var product = await _context.Product
                .Where(p => p.ProductType == ProductType.Event && p.OriginalId == enroll.EventId)
                .FirstOrDefaultAsync();
            if (product == null) return;

            var payment = await _context.Payment
                .Where(p => p.ProductId == product.Id && p.UserId == enroll.UserId)
                .FirstOrDefaultAsync();
            if (payment == null) return;

            payment.PaymentStatus = PaymentStatus.Canceled;

            _context.Payment.Update(payment);
            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Checks if club has a valid key property on their settings
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<bool> ClubHasValidKey(int clubId)
        {
            var club = await _context.ClubPaymentSettings.FindAsync(clubId);
            if (club == null) return false;
            return club.ValidCredentials;
        }

        /// <summary>
        /// Gets all payments for a given club
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Payment>> GetClubPayments(int clubId)
        {
            return await _context.Payment
                .Include(p => p.Product)
                .Include(p => p.User)
                .Where(p => p.Product.ClubId == clubId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the product info from a given club subscription plan
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public async Task<Product?> GetClubSubscriptionPlan(int planId, bool? includeDisabled = false)
        {
            var query = _context.Product.Where(p => p.ProductType == ProductType.ClubSubscription && p.Id == planId);

            if (includeDisabled != true)
            {
                query = query.Where(p => p.Enabled == true);
            }

            return await query.FirstOrDefaultAsync();
        }


        public async Task<Product> UpdateProduct(Product product)
        {
            _context.Product.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> AnySubscriptionUsingPlan(int planId)
        {
            return await _context.Subscription.Where(s => s.ProductId == planId).AnyAsync();
        }

        public async Task DeleteProduct(int productId)
        {
            var product = await _context.Product.FindAsync(productId);
            if (product == null) return;

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
