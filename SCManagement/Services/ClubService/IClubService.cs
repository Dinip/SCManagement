using SCManagement.Models;

namespace SCManagement.Services.ClubService
{
    public interface IClubService
    {
        public Task<IEnumerable<Club>> GetClubs();
        public Task<Club> GetClub(int id);
        public Task<Club> CreateClub(Club club);
        public Task<Club> UpdateClub(Club club);
        public Task<Club> DeleteClub(int id);
        public Task<CodeClub> GenerateCode(int clubId, string creatorId, int roleId, DateTime? expireDate);
        public Task<IEnumerable<RoleClub>> GetRoles();
        public bool IsClubManager(string userId, int clubId);
        public Task<IEnumerable<CodeClub>> GetCodes(int clubId);
        public Task<CodeClub> GetCodeWithInfos(int id);
        public bool IsAlreadyInAClub(string userId);
        public bool UseCode(string userId, string code);
    }
}
