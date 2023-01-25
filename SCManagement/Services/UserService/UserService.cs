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

        /// <summary>
        /// Saves the updated user to the database and refreshes the session
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UpdateUser(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);
        }

        /// <summary>
        /// Gets the user with the given id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User?> GetUser(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        /// <summary>
        /// Gets the user with the given id and includes the roles
        /// that the user has in the clubs
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User?> GetUserWithRoles(string userId)
        {
            return await _context.Users
                .Include(u => u.UsersRoleClub)
                .ThenInclude(r => r.Club)
                .Include(u => u.UsersRoleClub)
                .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Updates the default tuple (club + role) for the given user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="usersRoleClubId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the selected role for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>User role in a club</returns>
        public async Task<UsersRoleClub> GetSelectedRole(string userId)
        {
            var result = await _context.UsersRoleClub.FirstOrDefaultAsync(u => u.UserId == userId && u.Selected == true);
            return result ?? new UsersRoleClub { Id = 0, UserId = userId, ClubId = 0, RoleId = 0, Selected = false };
        }

        /// <summary>
        /// Checks if the give user is an athlete in any club
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<bool> IsAtleteInAnyClub(string userId)
        {
            return _context.UsersRoleClub.AnyAsync(u => u.UserId == userId && u.RoleId == 20);
        }
    }
}
