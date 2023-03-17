using Microsoft.AspNetCore.Identity;
using SCManagement.Models.Validations;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.PlansService.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Models
{
    public class User : IdentityUser
    {
        [PersonalData]
        [Required(ErrorMessage = "Error_Required")]
        [StringLength(100, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [PersonalData]
        [Required(ErrorMessage = "Error_Required")]
        [StringLength(100, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [PersonalData]
        [Display(Name = "Date Of Birth")]
        [DateOfBirth(MinAge = 6, MaxAge = 100, ErrorMessage = "Error_DateOfBirth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public int? ProfilePictureId { get; set; }
        public BlobDto? ProfilePicture { get; set; }

        public int? EMDId { get; set; }
        public BlobDto? EMD { get; set; }

        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        //later, this settings might end up on a individual 
        //class with notification settings
        [Required(ErrorMessage = "Error_Required")]
        public string Language { get; set; } = "en-US";

        [Required(ErrorMessage = "Error_Required")]
        public string Theme { get; set; } = "light";

        public ICollection<UsersRoleClub>? UsersRoleClub { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName
        {
            get => $"{FirstName} {LastName}";
        }

        [NotMapped]
        [Display(Name = "Age")]
        public int? Age
        {
            get
            {
                if (DateOfBirth.HasValue)
                {
                    var today = DateTime.Today;
                    var age = today.Year - DateOfBirth.Value.Year;
                    if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                    return age;
                }
                return null;
            }
        }

        public ICollection<TrainingPlan>? TrainingPlans { get; set; }
        public ICollection<MealPlan>? MealPlans { get; set; }
        public ICollection<Goal>? Goals { get; set; }
    }
}
