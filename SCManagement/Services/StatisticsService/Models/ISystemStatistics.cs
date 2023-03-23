using System.ComponentModel.DataAnnotations;

namespace SCManagement.Services.StatisticsService.Models
{
    public interface ISystemStatistics
    {
        public int Id { get; set; }

        public StatisticsRange StatisticsRange { get; set; }

        [DataType(DataType.Date)]
        public DateTime Timestamp { get; set; }
    }
}
