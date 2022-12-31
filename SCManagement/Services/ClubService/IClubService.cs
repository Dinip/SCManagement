
using SCManagement.Models;

namespace SCManagement.Services.ClubService
{
    /// <summary>
    /// club service interface
    /// </summary>
    public interface IClubService
    {
        public Task<IEnumerable<Club>> GetClubs();
        public Task<Club> GetClub(int id);
        public int GetClubId(int id);
        public Task<Club> CreateClub(Club club, string userId);
        public Task<Club> UpdateClub(Club club);
        public Task<Club> DeleteClub(int id);
        public Task<CodeClub> GenerateCode(int clubId, string creatorId, int roleId, DateTime? expireDate);
        public Task<IEnumerable<RoleClub>> GetRoles();
        public Task<IEnumerable<CodeClub>> GetCodes(int clubId);
        public Task<CodeClub> GetCodeWithInfos(int id);
        public bool UserAlreadyInAClub(string userId, int? clubId = null);
        public KeyValuePair<bool, string> UseCode(string userId, CodeClub code);
        public bool IsClubAdmin(string userId, int clubId);
        public bool IsClubSecretary(string userId, int clubId);
        public bool IsClubManager(string userId, int clubId);
        public bool IsClubTrainer(string userId, int clubId);
        public bool IsClubStaff(string userId, int clubId);
        public bool IsClubAthlete(string userId, int clubId);
        public bool IsClubMember(string userId, int clubId);
        public bool IsClubPartner(string userId, int clubId);
        public bool ApproveCode(string code);
        public Task SendCodeEmail(int codeId, string email, int clubId);
        public List<int> UserRolesInClub(string userId, int clubId);
        public IQueryable<UsersRoleClub> GetPartnerList(int clubId);
        public Task<IEnumerable<Modality>> GetModalities();
        public bool UserHasRoleInClub(string userId, int clubId, int roleId);

        //PROBLEMAS
        //public Task<IEnumerable<User>> GetPartnerListUsers(int clubId);

        //public void RemoveClubUser(string userId, int clubId, int roleId);

        //public void AddClubUser(string userId, int clubId, int roleId);

    }
}
