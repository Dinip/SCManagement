using SCManagement.Models;
using SCManagement.Services.PaymentService.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    public class SystemPlansStatistics : ISystemStatistics
    {
        public int Id { get; set; }
        public int Active { get; set; } = 0;
        public int Canceled { get; set; } = 0;

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public StatisticsRange StatisticsRange { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
