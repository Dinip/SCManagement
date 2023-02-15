using System.ComponentModel.DataAnnotations.Schema;
using SCManagement.Services.PaymentService.Models;

namespace SCManagement.Models
{
    public class ClubPaymentSettings
    {
        [ForeignKey("Club")]
        public int ClubPaymentSettingsId { get; set; }
        public Club? Club { get; set; }
        public string? AccountId { get; set; }
        public string? AccountKey { get; set; }
        public string RequestSecret { get; set; } = Guid.NewGuid().ToString();
        public SubscriptionFrequency? QuotaFrequency { get; set; }
        public float? QuotaFee { get; set; }
    }
}
