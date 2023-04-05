using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Services.PaymentService.Models
{
    [NotMapped]
    public class PayPayment
    {
        public int Id { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }
    }
}
