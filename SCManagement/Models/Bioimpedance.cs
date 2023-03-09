using System.ComponentModel.DataAnnotations;

namespace SCManagement.Models
{
    public class Bioimpedance
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        [Display(Name ="Height")]
        public double? Weight { get; set; }
        [Display(Name = "Weight")]
        public double? Height { get; set; }
        [Display(Name = "FatMass")]
        public double? FatMass { get; set; }
        [Display(Name = "LeanMass")]
        public double? LeanMass { get; set; }
        [Display(Name = "MuscleMass")]
        public double? MuscleMass { get; set; }
        [Display(Name = "ViceralFat")]
        public double? ViceralFat { get; set; }
        [Display(Name = "BasalMetabolism")]
        public double? BasalMetabolism { get; set; }
        [Display(Name = "Hydration")]
        public double? Hydration { get; set; }
        public DateTime LastUpdateDate { get; set; } = DateTime.Now;
    }
}
