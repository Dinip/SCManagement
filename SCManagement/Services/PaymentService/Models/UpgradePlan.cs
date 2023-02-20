using System.ComponentModel.DataAnnotations;

namespace SCManagement.Services.PaymentService.Models
{
    public class UpgradePlan
    {
        [Required]
        public int SubscriptionId { get; set; }
        [Required]
        public int PlanId { get; set; }

        public int? Athletes { get; set; }
        public ICollection<Product>? Plans { get; set; }
    }
}
