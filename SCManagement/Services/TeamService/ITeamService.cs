using SCManagement.Models;

namespace SCManagement.Services.TeamService
{
    public interface ITeamService
    {
        public Task<IEnumerable<Team>> GetTeams(int clubId);
        public Task<Team?> GetTeam(int teamId);

        public Task<Team> CreateTeam(Team team);
        public Task<Team> UpdateTeam(Team team);
        public Task UpdateTeamAthletes(int teamId, IEnumerable<string> atheltesId);

        public Task RemoveAthlete(Team team, User athlete);

        public Task DeleteTeam(Team team);

        public Task<IEnumerable<Team>> GetTeamsByAthlete(string userId);
        public Task<IEnumerable<Team>> GetTeamsByAthlete(string userId, int clubId);

        public Task<IEnumerable<Team>> GetTeamsByTrainer(string userId);
    }
}
