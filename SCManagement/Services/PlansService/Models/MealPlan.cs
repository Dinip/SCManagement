using SCManagement.Models;

namespace SCManagement.Services.PlansService.Models
{
    public class MealPlan : Plan
    {
        public ICollection<MealPlanSession>? MealPlanSessions { get; set; }
    }
}
