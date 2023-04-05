using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SCManagement.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    public interface IClubStatistics
    {
        public int Id { get; set; }

        public int ClubId { get; set; }
        public Club? Club { get; set; }

        public StatisticsRange StatisticsRange { get; set; }

        [DataType(DataType.Date)]
        public DateTime Timestamp { get; set; }
    }
}
