using SCManagement.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    public class ClubEventStatistic : IClubStatistic
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public int ClubId { get; set; }
        public Club? Club { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        public StatisticRange StatisticRange { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
