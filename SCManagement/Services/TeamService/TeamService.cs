using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Data.Migrations;
using SCManagement.Models;

namespace SCManagement.Services.TeamService
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task UpdateTeamAthletes(int teamId, IEnumerable<string> atheltesId)
        {
            var team = await _context.Team.Include(u => u.Athletes).FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null || !atheltesId.Any()) return;

            var athletesToAdd = await _context.UsersRoleClub
                .Where(r => atheltesId.Contains(r.UserId) && r.RoleId == 20 && r.ClubId == team.ClubId)
                .Select(r => r.User)
                .ToListAsync();

            foreach (var athlete in athletesToAdd)
            {
                if (!team.Athletes.Contains(athlete))
                    team.Athletes.Add(athlete);
            }

            _context.Team.Update(team);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAthlete(Team team, User athlete)
        {
            team.Athletes.Remove(athlete);
            _context.Team.Update(team);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeam(Team team)
        {
            _context.Team.Remove(team);
            await _context.SaveChangesAsync();
        }
    }
}
