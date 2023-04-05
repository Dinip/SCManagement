using System.ComponentModel.DataAnnotations.Schema;
using SCManagement.Services.PaymentService.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    [NotMapped]
    public class BackofficeStats
    {
        public MinMaxHelper Clubs { get; set; }
        public MinMaxHelper Income { get; set; }
        public MinMaxHelper Codes { get; set; }
        public Product BestSeller { get; set; }
        public MinMaxHelper Payments { get; set; }
    }

    public class MinMaxHelper
    {
        public float Min { get; set; } = 0;
        public float Max { get; set; } = 0;
    }
}
