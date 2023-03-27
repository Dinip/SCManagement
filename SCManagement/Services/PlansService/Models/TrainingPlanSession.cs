using System.ComponentModel.DataAnnotations;

namespace SCManagement.Services.PlansService.Models
{
    public class TrainingPlanSession
    {
        public int Id { get; set; }
        public int TrainingPlanId { get; set; }
        public TrainingPlan? TrainingPlan { get; set; }
        
        [Required(ErrorMessage = "Error_Required")]
        [StringLength(60, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Exercise Name")]
        public string ExerciseName { get; set; }

        [Required(ErrorMessage = "Error_Required")]
        [StringLength(60, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Exercise Description")]
        public string ExerciseDescription { get; set; }

        [Range(1,1000, ErrorMessage = "Error_MaxNumber")]
        [Display(Name = "Repetitions")]
        public int? Repetitions { get; set; }

        //Duration is in minutes
        [Range(1, 1000, ErrorMessage = "Error_MaxNumber")]
        [Display(Name = "Duration")]
        public int? Duration { get; set; }

    }
}
