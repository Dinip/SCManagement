using SCManagement.Models;

namespace SCManagement.Services.StatisticsService.Models
{
    public class ClubModalityStatistic : IClubStatistic
    {
        public int Id { get; set; }
        public int Value { get; set; }

        public int ModalityId { get; set; }
        public Modality? Modality { get; set; }

        public int ClubId { get; set; }
        public Club? Club { get; set; }

        public StatisticRange StatisticRange { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
