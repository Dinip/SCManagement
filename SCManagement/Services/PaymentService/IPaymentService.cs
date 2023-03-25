using SCManagement.Models;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.StatisticsService.Models;

namespace SCManagement.Services.PaymentService
{
    public interface IPaymentService
    {
        public Task<IEnumerable<Payment>> GetPayments(string userId);
        public Task<IEnumerable<Payment>> GetClubPayments(int clubId);
        public Task<Payment?> GetPayment(int id);

        public Task<Product?> GetProduct(int id);

        public Task WebhookHandleSinglePayment(PaymentWebhookGeneric data);
        public Task WebhookHandleSubscriptionCreate(PaymentWebhookGeneric data);
        public Task WebhookHandleSubscriptionPayment(PaymentWebhookGeneric data);

        public Task<IEnumerable<Subscription>> GetSubscriptions(string userId);
        public Task<Subscription?> GetSubscription(int id);

        public Task<IEnumerable<Product>> GetClubSubscriptionPlans();
        public Task<Product?> GetClubSubscriptionPlan(int planId);
        public Task<Subscription> SubscribeClubToPlan(int clubId, string userId, int planId);
        public Task UpgradeClubPlan(int subId, int newPlanId);

        public Task<Subscription?> SetSubscriptionToAuto(int subId);
        public Task<Subscription?> CancelAutoSubscription(int subId);
        public Task CancelSubscription(int id);

        public Task CreateProductEvent(Event myEvent);
        public Task UpdateProductEvent(Event myEvent, bool delete = false);

        public Task<Payment?> CreateEventPayment(EventEnroll enroll);
        public Task CancelEventPayment(EventEnroll enroll);
        public Task<Payment?> PaySinglePayment(PayPayment paymentInput);

        public Task UpdateProductClubMembership(ClubPaymentSettings clubPaymentSettings);
        public Task TestAccount(string id, string key);

        public Task<Subscription> CreateMembershipSubscription(UsersRoleClub partner);
        public Task<Subscription?> GetMembershipSubscription(string userId, int clubId);

        public Task<bool> ClubHasValidKey(int clubId);
    }
}
