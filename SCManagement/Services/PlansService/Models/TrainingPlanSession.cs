using System.ComponentModel.DataAnnotations;

namespace SCManagement.Services.PlansService.Models
{
    public class TrainingPlanSession
    {
        public int Id { get; set; }
        public int TrainingPlanId { get; set; }
        public TrainingPlan? TrainingPlan { get; set; }
        public string ExerciseName { get; set; }
        public string ExerciseDescription { get; set; }

        [Range(0,1000)]
        public int? Repetitions { get; set; }

        //Duration is in minutes
        [Range(0, 100)]
        public int? Duration { get; set; }

    }
}
