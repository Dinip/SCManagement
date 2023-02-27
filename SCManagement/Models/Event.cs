using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SCManagement.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Error_Required")]
        [StringLength(40, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Event Name")]
        public string Name { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        //Fazer validação: Tem de ser depois ou igual ao end date
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Event Details")]
        public string? Details { get; set; }
        [Display(Name = "Public Event")]
        public bool IsPublic { get; set; }
        [Display(Name = "Fee")]
        public double Fee { get; set; }
        [Display(Name = "Event Location")]
        public int? LocationId { get; set; }
        public Address? Location { get; set; }
        [Display(Name = "Event Route")]
        public bool HaveRoute { get; set; } = false;
        public string? Route { get; set; }
        [Display(Name = "Max Enrolls")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]

        //Fazer validação: tem de ser depois da start date e antes da end date
        public int MaxEventEnrolls { get; set; }
        [Display(Name = "Enrool Limit Date")]
        public DateTime EnroolLimitDate { get; set; }
        public ICollection<EventEnroll> UsersEnrooled { get; set; } = new List<EventEnroll>();
        public int ClubId { get; set; }
        [Display(Name = "Sponsor")]
        public Club? Club { get; set; }
        public ResultType EventResultType { get; set; }
        public ICollection<EventResult>? Results { get; set; }
        public enum ResultType : int
        {
            Time = 1,
            Position = 2,
            Score = 3
        }

        public string? AddressByPath { get; set; }
    }
}
