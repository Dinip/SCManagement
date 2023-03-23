using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Models
{
    public class Bioimpedance
    {
        [ForeignKey("User")]
        public string BioimpedanceId { get; set; }
        //public int Id { get; set; }
        //public string UserId { get; set; }
        //public User? User { get; set; }

        [Display(Name = "Height")]
        [RegularExpression("^((([1-9][0-9]{0,2})?'?([1-9][0-9]{0,2})(\"([1-9][0-9]{0,2})?|'')?)|([1-9][0-9]{0,2}((\\.|\\,)[0-9]{1,2})?(cm|m)))$",
            ErrorMessage = "Error_Bio_Height" )]
        public string? Height { get; set; }

        [Display(Name = "Weight")]
        [RegularExpression("^([1-9][0-9]{0,2}((\\.|\\,)[0-9]{1,2})?(kg|lb|lbs))$",
            ErrorMessage = "Error_Bio_Weight")]
        public string? Weight{ get; set; }

        [Display(Name = "FatMass")]
        [Range(0, 100)]
        public float? FatMass { get; set; }

        [Display(Name = "LeanMass")]
        [Range(0, 100)]
        public float? LeanMass { get; set; }

        [Display(Name = "MuscleMass")]
        [Range(0, 100)]
        public float? MuscleMass { get; set; }

        [Display(Name = "ViceralFat")]
        [Range(0, 60)]
        public float? ViceralFat { get; set; }

        [Display(Name = "BasalMetabolism")]
        public float? BasalMetabolism { get; set; }

        [Display(Name = "Hydration")]
        [Range(0, 100)]
        public float? Hydration { get; set; }

        public DateTime LastUpdateDate { get; set; } = DateTime.Now;
    }
}
