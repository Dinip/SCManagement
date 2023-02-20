using SCManagement.Models;
using SCManagement.Services.PaymentService.Models;

namespace SCManagement.Services.PaymentService
{
    public interface IPaymentService
    {
        public Task<IEnumerable<Payment>> GetPayments(string userId);
        public Task<Product?> GetProduct(int id);
        public Task<Payment?> CreatePayment(CreatePayment paymentInput, string userId);
        public Task UpdatePaymentFromWebhook(PaymentWebhook data);

        public Task WebhookHandleSinglePayment(PaymentWebhookGeneric data);
        public Task WebhookHandleSubscriptionCreate(PaymentWebhookGeneric data);
        public Task WebhookHandleSubscriptionPayment(PaymentWebhookGeneric data);

        public Task<Payment?> GetPayment(int id);
        public Task<Payment?> CreateSubscriptionPayment(CreatePayment paymentInput, string userId);
        public Task<IEnumerable<Subscription>> GetSubscriptions(string userId);
        public Task<Subscription?> GetSubscription(int id);

        public Task<IEnumerable<Product>> GetClubSubscriptionPlans();
        public Task<Subscription> SubscribeClubToPlan(int clubId, string userId, int planId);

        public Task<Subscription?> SetSubscriptionToAuto(int subId);
        public Task<Subscription?> CancelAutoSubscription(int subId);

        public Task CancelSubscription(int id);
        public Task UpgradeClubPlan(int subId, int newPlanId);
    }
}
