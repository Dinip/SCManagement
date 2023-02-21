using System.ComponentModel.DataAnnotations;

namespace SCManagement.Services.PaymentService.Models
{
    public class UpgradePlan
    {
        [Required]
        public int SubscriptionId { get; set; }

        [Required]
        [Display(Name = "Plan")]
        public int PlanId { get; set; }

        [Display(Name = "Athletes")]
        public int? Athletes { get; set; }

        [Display(Name = "Plans")]
        public ICollection<Product>? Plans { get; set; }
    }
}
