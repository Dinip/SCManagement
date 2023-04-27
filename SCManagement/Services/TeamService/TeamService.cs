﻿using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Data.Migrations;
using SCManagement.Models;
using SCManagement.Services.NotificationService;

namespace SCManagement.Services.TeamService
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Team service constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="notificationService"></param>
        public TeamService(ApplicationDbContext context,INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Gets a team by its id
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns>Found team or null</returns>
        public async Task<Team?> GetTeam(int teamId)
        {
            return await _context.Team
                .Include(t => t.Modality)
                .ThenInclude(m => m.ModalityTranslations)
                .Include(u => u.Trainer)
                .Include(u => u.Athletes)
                .FirstOrDefaultAsync(t => t.Id == teamId); ;
        }

        /// <summary>
        /// Gets all the teams for a given club
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns>List of teams</returns>
        public async Task<IEnumerable<Team>> GetTeams(int clubId)
        {
            return await _context.Team
                .Where(t => t.ClubId == clubId)
                .Include(t => t.Modality)
                .ThenInclude(m => m.ModalityTranslations)
                .Include(t => t.Trainer)
                .ToListAsync();
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

            List<string> userIds = new List<string>();
            foreach (var athlete in athletesToAdd)
            {
                if (!team.Athletes.Contains(athlete))
                {
                    team.Athletes.Add(athlete);
                    userIds.Add(athlete.Id);
                }
                    
            }

            _context.Team.Update(team);
            await _context.SaveChangesAsync();
            
            _notificationService.NotifyTeamAdded(team, userIds);
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

            _notificationService.NotifyTeam_Removed(team,athlete.Id);
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
        /// <returns></returns>
        public async Task<IEnumerable<Team>> GetTeamsByAthlete(string userId)
        {
            return await _context.Team
                .Where(t => t.Athletes.Any(a => a.Id == userId))
                .Include(t => t.Modality)
                .ThenInclude(m => m.ModalityTranslations)
                .Include(t => t.Trainer)
                .Include(c => c.Club)
                .Include(a => a.Athletes)
                .ToListAsync();
        }

        /// <summary>
        /// Get all the teams that a user belongs to in a club
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Team>> GetTeamsByAthlete(string userId, int clubId)
        {
            return await _context.Team
                .Where(t => t.ClubId == clubId && t.Athletes.Any(a => a.Id == userId))
                .Include(t => t.Modality)
                .ThenInclude(m => m.ModalityTranslations)
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
            return await _context.Team.Where(t => t.TrainerId == userId)
                .Include(t => t.Modality)
                .ThenInclude(m => m.ModalityTranslations)
                .Include(t => t.Trainer)
                .Include(t => t.Athletes)
                .ThenInclude(a => a.TrainingPlans)
                .Include(t => t.Athletes)
                .ThenInclude(a => a.MealPlans)
                .ToListAsync();
        }

        public async Task TransferOwnerOfAllTeams(string trainerId, string AdminId)
        {
            var teams = await GetTeamsByTrainer(trainerId);

            if(teams != null && teams.Count()> 0)
            {
                foreach(Team team in teams)
                {
                    team.TrainerId = AdminId;
                    _context.Team.Update(team);
                    await _context.SaveChangesAsync();
                }
            }

            
        }
    }
}
