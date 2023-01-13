﻿using SCManagement.Data.Migrations;
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
    }
}
