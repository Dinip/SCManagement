using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace SCManagement.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Error_Required")]
        [StringLength(40, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Team Name")]
        public string Name { get; set; }
        [Display(Name = "Date Created")]
        public DateTime CreationDate
        {
            get
            {
                return dateCreated.HasValue
                   ? dateCreated.Value
                   : DateTime.Now;
            }

            set { dateCreated = value; }
        }

        
        private DateTime? dateCreated = null;
        [Display(Name = "Modality")]
        public int ModalityId { get; set; }
        [Display(Name = "Modalities")]
        public Modality? Modality { get; set; }
        
        public ICollection<User>? Athletes { get; set; }
        
        public string TrainerId { get; set; }
        [Display(Name = "Trainer")]
        public User? Trainer { get; set; }
        public int ClubId { get; set; }
        public Club? Club { get; set; }
    }
}
