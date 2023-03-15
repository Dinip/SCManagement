using SCManagement.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    public class ClubUserStatistic : IClubStatistic
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public int RoleId { get; set; }
        public RoleClub? Role { get; set; }

        public int ClubId { get; set; }
        public Club? Club { get; set; }

        public StatisticRange StatisticRange { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
