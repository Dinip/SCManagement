using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Services.PlansService.Models
{
    public class MealPlanSession
    {
        public int Id { get; set; }
        public int MealPlanId { get; set; }
        public MealPlan? MealPlan { get; set; }
        public string MealName { get; set; }
        public string MealDescription { get; set; }
        public DateTime Time { get; set; }
    }
}
