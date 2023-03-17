using SCManagement.Services.PlansService.Models;

namespace SCManagement.Services.PlansService
{
    public interface IPlanService
    {
        public Task<IEnumerable<TrainingPlan?>> GetTrainingPlans(string trainerId);
        public Task<IEnumerable<TrainingPlan?>> GetTrainingPlans(string trainerId, string athleteId);
        public Task<IEnumerable<TrainingPlan?>> GetTemplateTrainingPlans(string trainerId);
        public Task<TrainingPlan?> GetTemplateTrainingPlan(int templateId);
        public Task<IEnumerable<MealPlan?>> GetMealPlans(string trainerId);
        public Task<IEnumerable<MealPlan?>> GetMealPlans(string trainerId, string athleteId);
        public Task<IEnumerable<MealPlan?>> GetTemplateMealPlans(string trainerId);
        public Task<MealPlan?> GetTemplateMealPlan(int templateId);
        public Task<TrainingPlan?> GetTrainingPlan(int planId);
        public Task<MealPlan?> GetMealPlan(int planId);
        public Task<TrainingPlan> CreateTrainingPlan(TrainingPlan plan);
        public Task<MealPlan> CreateMealPlan(MealPlan plan);
        public Task<TrainingPlan> UpdateTrainingPlan(TrainingPlan plan);
        public Task<MealPlan> UpdateMealPlan(MealPlan plan);
        public Task DeleteTrainingPlan(TrainingPlan plan);
        public Task DeleteMealPlan(MealPlan plan);
        public Task<IEnumerable<Goal?>> GetGoals(string trainerId);
        public Task<IEnumerable<Goal?>> GetGoals(string trainerId, string athleteId);
        public Task<Goal?> GetGoal(int goalId);
        public Task<Goal> CreateGoal(Goal goal);
        public Task<Goal> UpdateGoal(Goal goal);
        public Task DeleteGoal(Goal goal);
    }
}
