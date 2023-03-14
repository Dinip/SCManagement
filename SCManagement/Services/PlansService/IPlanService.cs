using SCManagement.Services.PlansService.Models;

namespace SCManagement.Services.PlansService
{
    public interface IPlanService
    {
        public Task<IEnumerable<TrainingPlan?>> GetTrainingPlans(string trainerId);
        public Task<IEnumerable<MealPlan?>> GetMealPlans(string trainerId);
        public Task<TrainingPlan?> GetTrainingPlan(int planId);
        public Task<MealPlan?> GetMealPlan(int planId);
        public Task<TrainingPlan> CreateTrainingPlan(TrainingPlan plan);
        public Task<MealPlan> CreateMealPlan(MealPlan plan);
        public Task<TrainingPlan> UpdateTrainingPlan(TrainingPlan plan);
        public Task<MealPlan> UpdateMealPlan(MealPlan plan);
        public Task DeleteTrainingPlan(TrainingPlan plan);
        public Task DeleteMealPlan(MealPlan plan);
    }
}
