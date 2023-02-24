using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SCManagement.Services.PaymentService.Models;

namespace SCManagement.Models
{
    public class ClubPaymentSettings
    {
        [ForeignKey("Club")]
        public int ClubPaymentSettingsId { get; set; }

        public Club? Club { get; set; }

        [Display(Name = "Account Id")]
        public string? AccountId { get; set; }

        [Display(Name = "Account Key")]
        public string? AccountKey { get; set; }

        [Display(Name = "Valid Credentials")]
        public bool ValidCredentials { get; set; } = false;

        [Display(Name = "Notification Secret")]
        public string RequestSecret { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "Quota Payment Frequency")]
        public SubscriptionFrequency QuotaFrequency { get; set; } = SubscriptionFrequency.Monthly;

        [Display(Name = "Quota Value")]
        public float QuotaFee { get; set; } = 0.0f;
    }
}
