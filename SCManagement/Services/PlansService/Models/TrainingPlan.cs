using SCManagement.Models;
using SCManagement.Models.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit.Abstractions;

namespace SCManagement.Services.PlansService.Models
{
    public class TrainingPlan : Plan
    {
        [Display(Name = "Modality")]
        public Modality? Modality { get; set; }
        public int ModalityId { get; set; }
        public ICollection<TrainingPlanSession>? TrainingPlanSessions { get; set; }
    }
}
