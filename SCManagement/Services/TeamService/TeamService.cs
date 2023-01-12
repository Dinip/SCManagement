using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
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

        public async Task<Team?> GetTeam(int teamId)
        {
            return await _context.Team.Include(t => t.Modality).Include(u => u.Trainer).Include(u => u.Athletes)
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

        public async Task<Team> UpdateTeam(Team team)
        {
            _context.Team.Update(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async void UpdateTeamAthletes(int teamId, IEnumerable<string> atheltesId)
        {
            Team? team = await GetTeam(teamId);
            
            if (team == null || atheltesId.Count() == 0) return;
            
            var athletesToAdd = await _context.UsersRoleClub.Where(u => u.UserId == atheltesId.First()).ToListAsync();

            foreach(var athlete in athletesToAdd)
            {
                if (!team.Athletes.Contains(athlete.User))
                    team.Athletes.Add(athlete.User);
            }
            _context.Team.Update(team);
            await _context.SaveChangesAsync();
        }
    }
}
