using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Services.StatisticsService.Models
{
    [NotMapped]
    public class SystemPlansShortStatistics
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Active { get; set; }
        public int Canceled { get; set; }
    }
}
