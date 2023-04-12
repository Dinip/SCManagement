﻿using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SCManagement.Models
{
    public class Event
    {
        public int Id { get; set; }
        

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        //Fazer validação: Tem de ser depois ou igual ao end date
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public ICollection<EventTranslation>? EventTranslations { get; set; }

        [Display(Name = "Public Event")]
        public bool IsPublic { get; set; }

        [Display(Name = "Fee")]
        [Range(0, float.MaxValue, ErrorMessage = "Please enter a value between 0 and 2147483647")]
        public float Fee { get; set; }

        public int? LocationId { get; set; }

        [Display(Name = "Event Location")]
        public Address? Location { get; set; }

        [Display(Name = "Event Have Route")]
        public bool HaveRoute { get; set; } = false;
        
        public string? Route { get; set; }

        [Display(Name = "Max Enrolls")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value between 0 and 2147483647")]
        public int MaxEventEnrolls { get; set; }

        //Fazer validação: tem de ser depois da start date e antes da end date
        [Display(Name = "Enroll Limit Date")]
        public DateTime EnrollLimitDate { get; set; }
        public ICollection<EventEnroll>? UsersEnrolled { get; set; }

        public int ClubId { get; set; }

        [Display(Name = "Sponsor")]
        public Club? Club { get; set; }

        [Display(Name = "Event Result Type")]
        public ResultType EventResultType { get; set; }

        public ICollection<EventResult>? Results { get; set; }
        [Display(Name= "Event Location")]
        public string? AddressByPath { get; set; }
        public DateTime CreationDate { get; set; }

    }

    public enum ResultType : int
    {
        [Display(Name = "ResultTime")]
        Time = 1,
        [Display(Name = "ResultPosition")]
        Position = 2,
        [Display(Name = "ResultScore")]
        Score = 3
    }
}
