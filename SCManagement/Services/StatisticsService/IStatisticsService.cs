﻿using SCManagement.Services.StatisticsService.Models;

namespace SCManagement.Services.StatisticsService
{
    public interface IStatisticsService
    {
        public Task CreateClubUserStatistics(int clubId);
        public Task CreateClubPaymentStatistics(int clubId);
        public Task CreateClubModalityStatistics(int clubId);
        public Task<ICollection<ClubCurrentUsers>> GetCurrentClubUsersStatistics(int clubId);
        public Task<ICollection<ClubPaymentStatistics>> GetClubPaymentStatistics(int clubId, int? year = null, int? month = null);
        public Task<ICollection<ClubPaymentStatistics>> GetClubPaymentDetailsStatistics(int clubId, int? year = null, int? month = null);
        public Task<ICollection<ClubUserStatistics>> GetClubUserStatistics(int clubId, int userTypeId, int? year = null, int? month = null);
        public Task<ICollection<ClubModalityStatistics>> GetClubModalityStatistics(int clubId, int? year = null, int? month = null);
    }
}