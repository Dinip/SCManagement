
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
        public Task<Club> CreateClub(Club club, string userId, int addressId);
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
        public int GetUserRoleInClub(string userId, int clubId);
        public Task<IEnumerable<Modality>> GetModalities();
        public bool UserHasRoleInClub(string userId, int clubId, int roleId);
        public Task<IEnumerable<UsersRoleClub>> GetClubPartners(int clubId);
        public Task UpdateClubPhoto(Club club, bool remove = false, IFormFile? file = null);
        public void UpdateClubModalities(Club club, IEnumerable<int> ModalitiesIds);
        public Task RemoveClubUser(int userRoleClubId);
        public Task RemoveClubUser(string userId, int clubId, int roleId);
        public Task<UsersRoleClub?> GetUserRoleClubFromId(int userRoleClubId);
        public Task AddUserToClub(string userId, int clubId, int roleId);
        public Task<int> GetAddressAsync(int countyId, string street, string zipCode, string number);
        public void UpdateClubAddress(int addressId, int CountyId, string Street, string ZipCode, string Number);
        public Task<IEnumerable<UsersRoleClub>> GetClubStaff(int clubId);
        public Task<IEnumerable<UsersRoleClub>> GetClubAthletes(int clubId);
    }
}
