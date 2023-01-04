using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;

namespace SCManagement.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsersRoleClub>> GetUserRoles(string userId)
        {
            return await _context.UsersRoleClub
                .Include(x => x.Club)
                .Include(x => x.Role)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
        
        public async Task UpdateUser(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
