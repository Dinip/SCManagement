using System.ComponentModel.DataAnnotations.Schema;
using SCManagement.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    [NotMapped]
    public class ClubGeneralInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ClubStatus ClubStatus { get; set; }
        public string SubscriptionName { get; set; }
        public DateTime StartDate { get; set; }
        public int Members { get; set; }
    }
}
