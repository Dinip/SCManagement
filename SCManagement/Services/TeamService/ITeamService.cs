using SCManagement.Models;

namespace SCManagement.Services.TeamService
{
    public interface ITeamService
    {

        public Task<IEnumerable<Team>> GetTeams(int clubId);
        public Task<Team> GetTeam(int teamId);

        public Task<Team> CreateTeam(Team team);
    }
}
