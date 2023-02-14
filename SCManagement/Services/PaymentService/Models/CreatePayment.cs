using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace SCManagement.Services.PaymentService.Models
{
    [NotMapped]
    public class CreatePayment
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        [Required]
        public bool AutoRenew { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
