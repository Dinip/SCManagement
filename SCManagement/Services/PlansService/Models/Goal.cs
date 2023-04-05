﻿using System.ComponentModel.DataAnnotations;
using SCManagement.Models;
using SCManagement.Models.Validations;

namespace SCManagement.Services.PlansService.Models
{
    public class Goal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Error_Required")]
        [StringLength(60, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Goal Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Error_Required")]
        [StringLength(300, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Goal Description")]
        public string Description { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DateGreaterThan(Model = "Goal")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Trainer")]
        public User? Trainer { get; set; }
        public string TrainerId { get; set; }

        [Display(Name = "Athlete")]
        public User? Athlete { get; set; }
        public string AthleteId { get; set; }
        public bool isCompleted { get; set; }
    }
}
