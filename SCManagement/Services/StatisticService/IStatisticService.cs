namespace SCManagement.Services.StatisticsService
{
    public interface IStatisticService
    {
        public Task CreateClubUserStatistic(int clubId);
        public Task CreateClubPaymentStatistic(int clubId);
    }
}
