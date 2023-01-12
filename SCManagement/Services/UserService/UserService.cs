using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;

namespace SCManagement.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;

        public UserService(ApplicationDbContext context,
            SignInManager<User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        public async Task UpdateUser(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);
        }

        public async Task<User> GetUser(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetUserWithRoles(string userId)
        {
            return await _context.Users
                .Include(u => u.UsersRoleClub)
                .ThenInclude(r => r.Club)
                .Include(u => u.UsersRoleClub)
                .ThenInclude(r => r.Role)
                .FirstAsync(u => u.Id == userId);
        }

        public async Task UpdateSelectedRole(string userId, int usersRoleClubId)
        {
            var currentSelected = await _context.UsersRoleClub.Where(u => u.UserId == userId && (u.Selected == true || u.Id == usersRoleClubId)).ToListAsync();
            foreach (var item in currentSelected)
            {
                //set selected for the specified role and the old selected role to false
                item.Selected = item.Id == usersRoleClubId;
                _context.Update(item);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<UsersRoleClub> GetSelectedRole(string userId)
        {
            var result = await _context.UsersRoleClub.FirstOrDefaultAsync(u => u.UserId == userId && u.Selected == true);
            return result ?? new UsersRoleClub { Id = 0, UserId = userId, ClubId = 0, RoleId = 0, Selected = false };
        }

        public Task<bool> IsAtleteInAnyClub(string userId)
        {
            return _context.UsersRoleClub.AnyAsync(u => u.UserId == userId && u.RoleId == 20);
        }
    }
}
