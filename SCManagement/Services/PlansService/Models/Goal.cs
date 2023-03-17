using System.ComponentModel.DataAnnotations;
using SCManagement.Models;
using SCManagement.Models.Validations;

namespace SCManagement.Services.PlansService.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DateGreaterThan(Model = "Goal")]
        public DateTime EndDate { get; set; }
        public User Trainer { get; set; }
        public string TrainerId { get; set; }
        public User Athlete { get; set; }
        public string AthleteId { get; set; }

    }
}
