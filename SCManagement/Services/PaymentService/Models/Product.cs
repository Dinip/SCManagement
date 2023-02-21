using System.ComponentModel.DataAnnotations;
using SCManagement.Models;

namespace SCManagement.Services.PaymentService.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Value")]
        public float Value { get; set; }

        [Display(Name = "Product Type")]
        public ProductType ProductType { get; set; }

        public int? OriginalId { get; set; }

        public int? ClubId { get; set; }

        [Display(Name = "Club")]
        public Club? Club { get; set; }

        [Display(Name = "Is Subscription")]
        public bool IsSubscription { get; set; } = false;

        [Display(Name = "Subscription Frequency")]
        public SubscriptionFrequency? Frequency { get; set; }

        public bool Enabled { get; set; } = true;

        [Display(Name = "Athlete Slots")]
        public int? AthleteSlots { get; set; }
    }

    public enum ProductType : int
    {
        Event = 1,
        ClubQuota = 2,
        ClubSubscription = 3
    }
}
