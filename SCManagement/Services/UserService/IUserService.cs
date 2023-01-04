using SCManagement.Models;

namespace SCManagement.Services.UserService
{
    public interface IUserService
    {
        public Task<IEnumerable<UsersRoleClub>> GetUserRoles(string userId);
        public Task UpdateUser(User user);
    }
}
