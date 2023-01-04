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

        public async Task<Team> GetTeam(int clubId, int teamId)
        {
            return await _context.Team.Include(t => t.Modality)
                .FirstOrDefaultAsync(t => t.Id == teamId && t.ClubId == clubId);
        }

        public async Task<IEnumerable<Team>> GetTeams(int clubId)
        {
            return await _context.Team.Where(t => t.ClubId == clubId).Include(t => t.Modality).ToListAsync();
        }

        public async Task<Team> CreateTeam(Team team, string userId, int clubId)
        {
            Team newTeam = new Team
            {
                Name = team.Name,
                ClubId = clubId,
                ModalityId = team.ModalityId,
                CreationDate = DateTime.Now,
                TrainerId = userId
            };

            _context.Team.Add(newTeam);
            await _context.SaveChangesAsync();

            return newTeam;
        }
    }
}
