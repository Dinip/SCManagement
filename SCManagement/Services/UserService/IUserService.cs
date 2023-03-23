using SCManagement.Models;

namespace SCManagement.Services.UserService
{
    public interface IUserService
    {
        public Task UpdateUser(User user);
        public Task<User?> GetUser(string userId);
        public Task<User?> GetUserWithRoles(string userId);
        public Task UpdateSelectedRole(string userId, int usersRoleClubId);
        public Task<UsersRoleClub> GetSelectedRole(string userId);
        public Task<bool> IsAtleteInAnyClub(string userId);
        public Task<Bioimpedance> CreateBioimpedance(Bioimpedance bioimpedance);
        public Task<IEnumerable<Bioimpedance>> GetBioimpedances(string userId);
        public Task<Bioimpedance> GetLastBioimpedance(string userId);
        public Task<Bioimpedance> UpdateBioimpedance(Bioimpedance bioimpedance);
        public Task<User> GetUserWithEMD (string userId);
        public Task CheckAndDeleteEMD(User user);
        public Task<bool> IsStaffInAnyClub(string userId);
        public Task<bool> IsTrainerInAnyClub(string userId);

    }
}
