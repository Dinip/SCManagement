﻿using System.ComponentModel.DataAnnotations;
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
        public DateTime EndDate { get; set; }
        public int ModalityId { get; set; }
        public Modality? Modality { get; set; }
        public ICollection<User>? Athletes { get; set; }
        
        public int TrainingId { get; set; }
        public User? Trainer { get; set; }
    }
}
