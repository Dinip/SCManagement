using System.Globalization;
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

        /// <summary>
        /// Gets a team by its id
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns>Found team or null</returns>
        public async Task<Team?> GetTeam(int teamId)
        {
            var team = await _context.Team
                .Include(t => t.Modality)
                .ThenInclude(m => m.ModalityTranslations)
                .Include(u => u.Trainer)
                .Include(u => u.Athletes)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null) return null;

            //Select only the modality in the current languague 
            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            team.Modality.ModalityTranslations = team.Modality.ModalityTranslations.Where(cc => cc.Language == cultureInfo).ToList();

            return team;
        }

        /// <summary>
        /// Gets all the teams for a given club
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns>List of teams</returns>
        public async Task<IEnumerable<Team>> GetTeams(int clubId)
        {
            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            return await _context.Team
                .Where(t => t.ClubId == clubId)
                .Include(t => t.Modality)
                .ThenInclude(m => m.ModalityTranslations)
                .Include(t => t.Trainer)
                .Select(t => new Team
                {
                    Id = t.Id,
                    Name = t.Name,
                    CreationDate = t.CreationDate,
                    ModalityId = t.ModalityId,
                    Modality = new Modality { Id = t.ModalityId, ModalityTranslations = t.Modality.ModalityTranslations.Where(cc => cc.Language == cultureInfo).ToList() },
                    Athletes = t.Athletes,
                    TrainerId = t.TrainerId,
                    ClubId = t.ClubId,
                    Trainer = t.Trainer,
                    Club = t.Club
                }).ToListAsync();
        }

        /// <summary>
        /// Persists the created team to the database
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public async Task<Team> CreateTeam(Team team)
        {
            _context.Team.Add(team);
            await _context.SaveChangesAsync();

            return team;
        }

        /// <summary>
        /// Updates the team in the database
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public async Task<Team> UpdateTeam(Team team)
        {
            _context.Team.Update(team);
            await _context.SaveChangesAsync();
            return team;
        }

        /// <summary>
        /// Adds one or more athlete to a give team team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="atheltesId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes an athlete from a team
        /// </summary>
        /// <param name="team"></param>
        /// <param name="athlete"></param>
        /// <returns></returns>
        public async Task RemoveAthlete(Team team, User athlete)
        {
            team.Athletes.Remove(athlete);
            _context.Team.Update(team);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a team from the database
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public async Task DeleteTeam(Team team)
        {
            _context.Team.Remove(team);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Get all the teams that a user belongs to
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Team>> GetTeamsByAthlete(string userId, int clubId)
        {
            return await _context.Team
                .Where(t => t.ClubId == clubId && t.Athletes.Any(a => a.Id == userId))
                .Include(t => t.Modality)
                .ThenInclude(t => t.ModalityTranslations)
                .Include(t => t.Trainer)
                .Include(c => c.Club)
                .Include(a => a.Athletes)
                .ToListAsync();
        }

        /// <summary>
        /// Get all the teams that are coached by the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Team>> GetTeamsByTrainer(string userId)
        {
            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            return await _context.Team.Where(t => t.TrainerId == userId)
                .Include(t => t.Modality)
                .ThenInclude(m => m.ModalityTranslations)
                .Include(t => t.Trainer)
                .Include(t => t.Athletes)
                .ThenInclude(a => a.TrainingPlans)
                .Include(t => t.Athletes)
                .ThenInclude(a => a.MealPlans)
                .Select(t => new Team
                {
                    Id = t.Id,
                    Name = t.Name,
                    CreationDate = t.CreationDate,
                    ModalityId = t.ModalityId,
                    Modality = new Modality { Id = t.ModalityId, ModalityTranslations = t.Modality.ModalityTranslations.Where(cc => cc.Language == cultureInfo).ToList() },
                    Athletes = t.Athletes,
                    TrainerId = t.TrainerId,
                    ClubId = t.ClubId,
                    Trainer = t.Trainer,
                    Club = t.Club
                })
                .ToListAsync();
        }
    }
}
