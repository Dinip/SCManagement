using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;

namespace SCManagement.Services.TeamService
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;
        private readonly SharedResourceService _sharedResource;

        public TeamService(ApplicationDbContext context, SharedResourceService sharedResource)
        {
            _context = context;
            _sharedResource = sharedResource;
        }

        public async Task<Team> GetTeam(int teamId)
        {
            return await _context.Team.Include(t => t.Modality)
                .FirstOrDefaultAsync(t => t.Id == teamId);
        }

        public async Task<IEnumerable<Team>> GetTeams(int clubId)
        {
            return await _context.Team.Where(t => t.ClubId == clubId).Include(t => t.Modality).Include(t => t.Trainer).ToListAsync();
        }

        public async Task<Team> CreateTeam(Team team)
        {
            _context.Team.Add(team);
            await _context.SaveChangesAsync();

            return team;
        }
    }
}
