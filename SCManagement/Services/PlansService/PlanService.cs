﻿using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Services.PlansService.Models;

namespace SCManagement.Services.PlansService
{
    public class PlanService : IPlanService
    {
        private readonly ApplicationDbContext _context;

        public PlanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TrainingPlan> CreateTrainingPlan(TrainingPlan plan)
        {
            _context.TrainingPlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }
        public async Task<MealPlan> CreateMealPlan(MealPlan plan)
        {
            _context.MealPlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<IEnumerable<TrainingPlan?>> GetTrainingPlans(string trainerId)
        {
            return await _context.TrainingPlans.Where(p => p.TrainerId == trainerId && p.IsTemplate == false)
                .Include(p => p.Modality)
                .ThenInclude(p => p.ModalityTranslations)
                .Include(p => p.Trainer)
                .Include(p => p.TrainingPlanSessions)
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainingPlan?>> GetTrainingPlans(string trainerId, string athleteId)
        {
            return await _context.TrainingPlans.Where(p => p.TrainerId == trainerId && p.AthleteId == athleteId && p.IsTemplate == false)
                .Include(p => p.Modality)
                .ThenInclude(p => p.ModalityTranslations)
                .Include(p => p.Trainer)
                .Include(p => p.TrainingPlanSessions)
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainingPlan?>> GetMyTrainingPlans(string userId, int? filter = 0)
        {
            var plansQuery = _context.TrainingPlans
                .Where(p => p.AthleteId == userId && p.IsTemplate == false);

            switch (filter)
            {
                //Return futures
                case 1:
                    plansQuery = plansQuery.Where(p => p.StartDate > DateTime.Now);
                    break;

                //Return Finished
                case 2:
                    plansQuery = plansQuery.Where(p => p.EndDate < DateTime.Now);
                    break;

                //Atives + Futures
                default:
                    plansQuery = plansQuery.Where(p => p.EndDate >= DateTime.Now);
                    break;
            }

            return await plansQuery
                .Include(p => p.Modality)
                .ThenInclude(p => p.ModalityTranslations)
                .Include(p => p.Trainer)
                .Include(p => p.TrainingPlanSessions)
                .OrderBy(p => p.StartDate)
                .Take(15)
                .ToListAsync();
        }

        public async Task<IEnumerable<MealPlan?>> GetMealPlans(string trainerId, string athleteId)
        {
            return await _context.MealPlans.Where(p => p.TrainerId == trainerId && p.AthleteId == athleteId && p.IsTemplate == false)
                .Include(p => p.Trainer)
                .Include(p => p.MealPlanSessions)
                .ToListAsync();
        }

        public async Task<IEnumerable<MealPlan?>> GetMealPlans(string trainerId)
        {
            return await _context.MealPlans.Where(p => p.TrainerId == trainerId && p.IsTemplate == false)
                .Include(p => p.Trainer)
                .Include(p => p.MealPlanSessions)
                .ToListAsync();
        }

        public async Task<IEnumerable<MealPlan>> GetMyMealPlans(string userId, int? filter = 0)
        {
            var now = DateTime.Now;
            var query = _context.MealPlans.Where(p => p.AthleteId == userId && !p.IsTemplate);

            switch (filter)
            {
                //Return futures
                case 1:
                    query = query.Where(p => p.StartDate > now);
                    break;

                //Return Finished
                case 2:
                    query = query.Where(p => p.EndDate < now);
                    break;

                //Atives + Futures
                default:
                    query = query.Where(p => p.EndDate >= now);
                    break;
            }

            return await query
                .Include(p => p.MealPlanSessions)
                .Include(p => p.Trainer)
                .OrderByDescending(p => p.StartDate)
                .Take(15)
                .ToListAsync();
        }

        public async Task<TrainingPlan?> GetTrainingPlan(int planId)
        {
            return await _context.TrainingPlans.Where(p => p.Id == planId)
                .Include(p => p.Modality)
                .ThenInclude(p => p.ModalityTranslations)
                .Include(p => p.Trainer)
                .Include(p => p.TrainingPlanSessions)
                .FirstOrDefaultAsync();
        }

        public async Task<MealPlan?> GetMealPlan(int planId)
        {
            return await _context.MealPlans.Where(p => p.Id == planId)
                .Include(p => p.Trainer)
                .Include(p => p.MealPlanSessions)
                .FirstOrDefaultAsync();
        }

        public async Task<TrainingPlan> UpdateTrainingPlan(TrainingPlan plan)
        {
            _context.TrainingPlans.Update(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<MealPlan> UpdateMealPlan(MealPlan plan)
        {
            _context.MealPlans.Update(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public Task DeleteTrainingPlan(TrainingPlan plan)
        {
            _context.TrainingPlans.Remove(plan);
            return _context.SaveChangesAsync();
        }

        public Task DeleteMealPlan(MealPlan plan)
        {
            _context.MealPlans.Remove(plan);
            return _context.SaveChangesAsync();
        }

        public async Task<TrainingPlan?> GetTemplateTrainingPlan(int templateId)
        {
            return await _context.TrainingPlans.Where(p => p.Id == templateId && p.IsTemplate == true)
                .Include(p => p.Modality)
                .ThenInclude(p => p.ModalityTranslations)
                .Include(p => p.Trainer)
                .Include(p => p.TrainingPlanSessions)
                .FirstAsync();
        }

        public async Task<MealPlan?> GetTemplateMealPlan(int templateId)
        {
            return await _context.MealPlans.Where(p => p.Id == templateId && p.IsTemplate == true)
                .Include(p => p.Trainer)
                .Include(p => p.MealPlanSessions)
                .FirstAsync();
        }

        public async Task<IEnumerable<TrainingPlan?>> GetTemplateTrainingPlans(string trainerId)
        {
            return await _context.TrainingPlans.Where(p => p.TrainerId == trainerId && p.IsTemplate == true)
                .Include(p => p.Modality)
                .ThenInclude(p => p.ModalityTranslations)
                .Include(p => p.Trainer)
                .Include(p => p.TrainingPlanSessions)
                .ToListAsync();
        }
        public async Task<IEnumerable<MealPlan?>> GetTemplateMealPlans(string trainerId)
        {
            return await _context.MealPlans.Where(p => p.TrainerId == trainerId && p.IsTemplate == true)
                .Include(p => p.Trainer)
                .Include(p => p.MealPlanSessions)
                .ToListAsync();
        }

        public async Task<IEnumerable<Goal?>> GetGoals(string trainerId)
        {
            return await _context.Goals.Where(g => g.TrainerId == trainerId).ToListAsync();
        }
        public async Task<IEnumerable<Goal?>> GetGoals(string trainerId, string athleteId)
        {
            return await _context.Goals.Where(g => g.TrainerId == trainerId && g.AthleteId == athleteId).ToListAsync();
        }
        public async Task<Goal?> GetGoal(int goalId)
        {
            return await _context.Goals.Where(g => g.Id == goalId).FirstOrDefaultAsync();
        }
        public async Task<Goal> CreateGoal(Goal goal)
        {
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
            return goal;
        }
        public async Task<Goal> UpdateGoal(Goal goal)
        {
            _context.Goals.Update(goal);
            await _context.SaveChangesAsync();
            return goal;
        }
        public async Task DeleteGoal(Goal goal)
        {
            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Goal?>> GetMyGoals(string userId, int? filter)
        {
            switch (filter)
            {
                case 1:
                    return await _context.Goals.Where(g => g.AthleteId == userId && g.isCompleted == false).ToListAsync();

                case 2:
                    return await _context.Goals.Where(g => g.AthleteId == userId && g.isCompleted == true).ToListAsync();

                case 3:
                    return await _context.Goals.Where(g => g.AthleteId == userId).ToListAsync();
                default:
                    return await _context.Goals.Where(g => g.AthleteId == userId && g.EndDate > DateTime.Now).ToListAsync();
            }

        }
    }
}
