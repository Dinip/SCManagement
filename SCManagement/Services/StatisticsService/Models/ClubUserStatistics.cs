using SCManagement.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    public class ClubUserStatistics : IClubStatistics
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public int RoleId { get; set; }
        public RoleClub? Role { get; set; }

        public int ClubId { get; set; }
        public Club? Club { get; set; }

        public StatisticsRange StatisticsRange { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
