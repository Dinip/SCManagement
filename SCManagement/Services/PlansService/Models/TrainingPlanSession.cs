namespace SCManagement.Services.PlansService.Models
{
    public class TrainingPlanSession
    {
        public int Id { get; set; }
        public int TrainingPlanId { get; set; }
        public TrainingPlan? TrainingPlan { get; set; }
        public string ExerciseName { get; set; }
        public string ExerciseDescription { get; set; }
        public int? Repetitions { get; set; }
        
        //Duration is in minutes
        public int? Duration { get; set; }

    }
}
