using SCManagement.Models;

namespace SCManagement.Services.PaymentService.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
        public ProductType ProductType { get; set; }
        public int? OriginalId { get; set; }
        public int? ClubId { get; set; }
        public Club? Club { get; set; }
        public bool IsSubscription { get; set; } = false;
        public SubscriptionFrequency? Frequency { get; set; }
        public bool Enabled { get; set; } = true;
    }

    public enum ProductType : int
    {
        Event = 1,
        ClubQuota = 2,
        ClubSubscription = 3
    }
}
