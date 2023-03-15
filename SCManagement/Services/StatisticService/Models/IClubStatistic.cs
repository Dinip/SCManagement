using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SCManagement.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    public interface IClubStatistic
    {
        public int Id { get; set; }

        public int ClubId { get; set; }
        public Club? Club { get; set; }

        public StatisticRange StatisticRange { get; set; }

        [DataType(DataType.Date)]
        public DateTime Timestamp { get; set; }
    }

    public enum StatisticRange : int
    {
        Year = 1,
        Month = 2,
        Week = 3,
        Day = 4,
    }
}
