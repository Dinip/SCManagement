using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Models
{
    public class Bioimpedance
    {
        [ForeignKey("User")]
        public string BioimpedanceId { get; set; }

        [Display(Name = "Height")]
        [RegularExpression("^((([1-9][0-9]{0,2})?'?([1-9][0-9]{0,2})(\"([1-9][0-9]{0,2})?|'')?)|([1-9][0-9]{0,2}((\\.|\\,)[0-9]{1,2})?(cm|m)))$")]
        public string? Weight { get; set; }

        [Display(Name = "Weight")]
        [RegularExpression("^([1-9][0-9]{0,2}((\\.|\\,)[0-9]{1,2})?(kg|lb|lbs))$")]
        public string? Height { get; set; }

        [Display(Name = "FatMass")]
        public float? FatMass { get; set; }

        [Display(Name = "LeanMass")]
        public float? LeanMass { get; set; }

        [Display(Name = "MuscleMass")]
        public float? MuscleMass { get; set; }

        [Display(Name = "ViceralFat")]
        public float? ViceralFat { get; set; }

        [Display(Name = "BasalMetabolism")]
        public float? BasalMetabolism { get; set; }

        [Display(Name = "Hydration")]
        public float? Hydration { get; set; }

        public DateTime LastUpdateDate { get; set; } = DateTime.Now;
    }
}
