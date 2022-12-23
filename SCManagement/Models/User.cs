using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Resources;

namespace SCManagement.Models
{
    public class User : IdentityUser
    {
        [PersonalData]
        [Required(ErrorMessage = "Error_Required")]
        [StringLength(100, ErrorMessage = "Error_Legth", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [PersonalData]
        [Required(ErrorMessage = "Error_Required")]
        [StringLength(100, ErrorMessage = "Error_Legth", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [PersonalData]
        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        [PersonalData]
        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        //later, this settings might end up on a individual 
        //class with notification settings
        [Required(ErrorMessage = "Error_Required")]
        public string Language { get; set; } = "en-US";

        [Required(ErrorMessage = "Error_Required")]
        public string Theme { get; set; } = "light";
    }
}
