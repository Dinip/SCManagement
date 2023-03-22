using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Services.StatisticsService.Models
{
    [NotMapped]
    public class ClubStatisticsAggregate
    {
        public ICollection<ClubCurrentUsers> ClubCurrentUsers { get; set; } = new List<ClubCurrentUsers>();
        public ICollection<ClubPaymentStatistics> ClubPaymentStatistics { get; set; } = new List<ClubPaymentStatistics>();
        public ICollection<ClubModalityStatistics> ClubModalityStatistics { get; set; } = new List<ClubModalityStatistics>();
        public ICollection<ClubUserStatistics> ClubUserStatistics { get; set; } = new List<ClubUserStatistics>();
    }
}
