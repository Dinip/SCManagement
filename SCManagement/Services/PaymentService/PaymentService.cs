using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.PaymentService.Models;

namespace SCManagement.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        public PaymentService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("AccountId", configuration["Easypay-SystemAccountId"]);
            _httpClient.DefaultRequestHeaders.Add("ApiKey", configuration["Easypay-SystemAccountKey"]);
        }

        /// <summary>
        /// Get all payments from a user ordered by date
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

        private async Task<EasypayResponse?> SubscriptionApiRequest(CreatePayment paymentInput, Product product, User user, DateTime startTime)
        {
            var result = await _httpClient.PostAsJsonAsync("https://api.test.easypay.pt/2.0/subscription", new
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
                    phone = paymentInput.PhoneNumber ?? user.PhoneNumber ?? ""
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

        private async Task<EasypayResponse?> SinglePaymentApiRequest(CreatePayment paymentInput, Product product, User user)
        {
            var result = await _httpClient.PostAsJsonAsync("https://api.test.easypay.pt/2.0/single", new
            {
                type = "sale",
                currency = "EUR",
                value = product.Value,
                method = Payment.ConvertMethod(paymentInput.PaymentMethod),
                capture = new
                {
                    descriptive = product.Name
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

        public async Task<Payment?> CreateSubscriptionPayment(CreatePayment paymentInput, string userId)
        {
            //verificar a que clube pertence o produto (no caso de evento/mensalidade) e ir buscar a api key do clube

            string guid = Guid.NewGuid().ToString();
            var product = await GetProduct(paymentInput.ProductId);
            if (product == null) return null;

            var user = await _context.Users.FindAsync(userId);

            var timeNowPlus2Min = DateTime.Now.AddMinutes(2);

            var content = await SubscriptionApiRequest(paymentInput, product, user, timeNowPlus2Min);
            if (content == null) return null;

            //var cardInfo = await _context.CardInfo.FirstOrDefaultAsync(f =>
            //    f.UserId == userId &&
            //    f.LastFourDigits == content.method.last_four &&
            //    f.Type == content.method.type &&
            //    f.ExpirationDate == content.method.expiration_date
            //);

            //if (cardInfo == null)
            //{
            //    cardInfo = new CardInfo
            //    {
            //        LastFourDigits = content.method.last_four,
            //        Type = content.method.type,
            //        ExpirationDate = content.method.expiration_date,
            //        UserId = userId
            //    };
            //    _context.CardInfo.Add(cardInfo);
            //}

            var sub = new Subscription
            {
                StartTime = DateTime.Now,
                NextTime = DateTime.Now.Add(Subscription.AddTime(product.Frequency)),
                Value = product.Value,
                ProductId = paymentInput.ProductId,
                UserId = userId,
                AutoRenew = true,
                Frequency = product.Frequency.Value,
                SubscriptionKey = content.id,
            };
            _context.Subscription.Add(sub);
            await _context.SaveChangesAsync();

            var payment = new Payment
            {
                ProductId = paymentInput.ProductId,
                Value = product.Value,
                PaymentMethod = paymentInput.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                UserId = userId,
                PaymentKey = content.id,
                Url = content.method.url,
                PhoneNumber = paymentInput.PaymentMethod == PaymentMethod.MbWay ? paymentInput.PhoneNumber : null,
                SubscriptionId = sub.Id,
            };
            _context.Payment.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> CreatePayment(CreatePayment paymentInput, string userId)
        {
            //verificar a que clube pertence o produto (no caso de evento/mensalidade) e ir buscar a api key do clube

            var product = await GetProduct(paymentInput.ProductId);
            if (product == null) return null;

            var user = await _context.Users.FindAsync(userId);

            var content = await SinglePaymentApiRequest(paymentInput, product, user);
            if (content == null) return null;

            Subscription? sub = null;

            if (product.IsSubscription)
            {
                sub = new Subscription
                {
                    StartTime = DateTime.Now,
                    NextTime = DateTime.Now.AddMonths(1),
                    Value = product.Value,
                    Status = SubscriptionStatus.Active,
                    ProductId = paymentInput.ProductId,
                    UserId = userId,
                    AutoRenew = false,
                    Frequency = product.Frequency.Value,
                    CardInfoData = buildCardInfo(content)
                };
                _context.Subscription.Add(sub);
                await _context.SaveChangesAsync();
            }

            var payment = new Payment
            {
                ProductId = paymentInput.ProductId,
                Value = product.Value,
                PaymentMethod = paymentInput.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                UserId = userId,
                PaymentKey = content.id,
                MbEntity = content.method.entity,
                MbReference = content.method.reference,
                Url = content.method.url,
                PhoneNumber = paymentInput.PaymentMethod == PaymentMethod.MbWay ? paymentInput.PhoneNumber : null,
                SubscriptionId = sub?.Id,
                CardInfoData = buildCardInfo(content)
            };
            _context.Payment.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdatePaymentFromWebhook(PaymentWebhook data)
        {
            var payment = await _context.Payment.FirstOrDefaultAsync(f => f.PaymentKey == data.key);
            if (payment == null) return;
            payment.PayedAt = DateTime.Now;
            payment.PaymentStatus = data.value >= payment.Value ? PaymentStatus.Paid : PaymentStatus.Pending;
            _context.Payment.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetPayment(int id)
        {
            return await _context.Payment
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptions(string userId)
        {
            return await _context.Subscription
                .Include(s => s.Product)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.NextTime)
                .ToListAsync();
        }

        public async Task<Subscription?> GetSubscription(int id)
        {
            return await _context.Subscription
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        //fazer uma funçao que recebe a key do pagamento / subscricao, vai buscar
        //a info da entitade, verifica de quem é o produto (clube ou sistema) e
        //vai buscar as api keys do clube ou do sistema e define no httpclient

        private async Task<EasypayResponse?> singlePaymentIdApiRequest(string id)
        {
            var result = await _httpClient.GetAsync($"https://api.test.easypay.pt/2.0/single/{id}");

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Error in singlePaymentIdApiRequest");
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return null;
            }

            return await result.Content.ReadFromJsonAsync<EasypayResponse>();
        }

        private async Task<EasypayResponse?> subscriptionPaymentIdApiRequest(string id)
        {
            var result = await _httpClient.GetAsync($"https://api.test.easypay.pt/2.0/subscription/{id}");

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine("Error in subscriptionPaymentIdApiRequest");
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                return null;
            }

            return await result.Content.ReadFromJsonAsync<EasypayResponse>();
        }


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

        public async Task WebhookHandleSinglePayment(PaymentWebhookGeneric data)
        {
            var payment = await _context.Payment.FirstOrDefaultAsync(p => p.PaymentKey == data.id);
            if (payment == null) return;

            var info = await singlePaymentIdApiRequest(data.id);
            if (info == null) return;

            if (info.payment_status != null) payment.PaymentStatus = Payment.ConvertStatus(info.payment_status);
            if (payment.PaymentStatus == PaymentStatus.Paid) payment.PayedAt = DateTime.Now;

            payment.CardInfoData = buildCardInfo(info);

            _context.Payment.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task WebhookHandleSubscriptionCreate(PaymentWebhookGeneric data)
        {
            var subscription = await _context.Subscription.FirstOrDefaultAsync(s => s.SubscriptionKey == data.id);
            if (subscription == null) return;

            var info = await subscriptionPaymentIdApiRequest(data.id);
            if (info == null) return;

            subscription.CardInfoData = buildCardInfo(info);
            subscription.Status = Subscription.ConvertStatus(info.method.status);

            _context.Subscription.Update(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task WebhookHandleSubscriptionPayment(PaymentWebhookGeneric data)
        {
            var subscription = await _context.Subscription.FirstOrDefaultAsync(s => s.SubscriptionKey == data.id);
            if (subscription == null) return;

            var info = await subscriptionPaymentIdApiRequest(data.id);
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

            subscription.NextTime = DateTime.Now.Add(Subscription.AddTime(subscription.Frequency));
            subscription.CardInfoData = buildCardInfo(info);
            subscription.Status = Subscription.ConvertStatus(info.method.status);

            _context.Subscription.Update(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
