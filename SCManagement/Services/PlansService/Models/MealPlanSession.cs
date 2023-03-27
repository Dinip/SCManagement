using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Services.PlansService.Models
{
    public class MealPlanSession
    {
        public int Id { get; set; }
        public int MealPlanId { get; set; }
        public MealPlan? MealPlan { get; set; }
        
        [Required(ErrorMessage = "Error_Required")]
        [StringLength(60, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Meal Name")]
        public string MealName { get; set; }

        [Required(ErrorMessage = "Error_Required")]
        [StringLength(300, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Meal Description")]
        public string MealDescription { get; set; }
        
        [DataType(DataType.Time)]
        [Display(Name = "Time")]
        public TimeSpan Time { get; set; }
    }
}
