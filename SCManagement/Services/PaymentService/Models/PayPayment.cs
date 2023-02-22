using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace SCManagement.Services.PaymentService.Models
{
    [NotMapped]
    public class PayPayment
    {
        public int Id { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
