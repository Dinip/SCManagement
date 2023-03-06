using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SCManagement.Models;

namespace SCManagement.Services.PaymentService.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Next Payment Date")]
        public DateTime NextTime { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndTime { get; set; }

        [Display(Name = "Value")]
        public float Value { get; set; }

        public int ProductId { get; set; }

        [Display(Name = "Product")]
        public Product? Product { get; set; }
        
        public string UserId { get; set; }
        
        public User? User { get; set; }

        [Display(Name = "Auto Renew")]
        public bool AutoRenew { get; set; } = false;

        [Display(Name = "Subscription Frequency")]
        public SubscriptionFrequency Frequency { get; set; }

        [Display(Name = "Subscription Status")]
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;
        public string? SubscriptionKey { get; set; }

        public string? CardInfoData { get; set; }

        [NotMapped]
        [Display(Name = "Card Info")]
        public CardInfo? CardInfo
        {
            get
            {
                return string.IsNullOrEmpty(CardInfoData) ? null : JsonConvert.DeserializeObject<CardInfo>(CardInfoData);
            }
        }

        public int? ClubId { get; set; }

        [Display(Name = "Club")]
        public Club? Club { get; set; }

        [Display(Name = "Url")]
        public string? ConfigUrl { get; set; }

        public static string ConvertFrequency(SubscriptionFrequency? frequency)
        {
            if (frequency == SubscriptionFrequency.Daily) return "1D";
            if (frequency == SubscriptionFrequency.Weekly) return "1W";
            if (frequency == SubscriptionFrequency.Monthly) return "1M";
            if (frequency == SubscriptionFrequency.Quarterly) return "3M";
            if (frequency == SubscriptionFrequency.Semiannually) return "6M";
            if (frequency == SubscriptionFrequency.Yearly) return "1Y";
            return "";
        }

        public static SubscriptionStatus ConvertStatus(string status)
        {
            if (status == "waiting") return SubscriptionStatus.Waiting;
            if (status == "pending") return SubscriptionStatus.Pending;
            if (status == "active") return SubscriptionStatus.Active;
            return SubscriptionStatus.Canceled;
        }

        public static TimeSpan AddTime(SubscriptionFrequency? frequency)
        {
            if (frequency == SubscriptionFrequency.Daily) return TimeSpan.FromDays(1);
            if (frequency == SubscriptionFrequency.Weekly) return TimeSpan.FromDays(7);
            if (frequency == SubscriptionFrequency.Monthly) return TimeSpan.FromDays(30);
            if (frequency == SubscriptionFrequency.Quarterly) return TimeSpan.FromDays(90);
            if (frequency == SubscriptionFrequency.Semiannually) return TimeSpan.FromDays(180);
            if (frequency == SubscriptionFrequency.Yearly) return TimeSpan.FromDays(365);
            return TimeSpan.Zero;
        }

        public override string ToString()
        {
            return $"Subscription {Id} - {UserId} - {ProductId} - {StartTime} - {NextTime} - {EndTime} - {Value} - {AutoRenew} - {Frequency} - {Status} - {SubscriptionKey} - {CardInfoData} - {ClubId} - {ConfigUrl}";
        }
    }

    public enum SubscriptionFrequency : int
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Quarterly = 4,
        Semiannually = 5,
        Yearly = 6
    }

    public enum SubscriptionStatus : int
    {
        Waiting = 1,
        Pending = 2,
        Active = 3,
        Canceled = 4,
        Pending_Cancel = 5
    }
}
