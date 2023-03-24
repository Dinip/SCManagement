using SCManagement.Models;
using SCManagement.Services.PaymentService.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    public class SystemPaymentStatistics : ISystemStatistics
    {
        public int Id { get; set; }
        public float Value { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public ProductType ProductType { get; set; }

        public StatisticsRange StatisticsRange { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
