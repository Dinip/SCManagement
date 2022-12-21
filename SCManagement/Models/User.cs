using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace SCManagement.Models
{
    public class User : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        
        [PersonalData]
        public string LastName { get; set; }

        [PersonalData]
        public string? ProfilePicture { get; set; }

        [PersonalData]
        public DateTime? DateOfBirth { get; set; }

        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        //later, this settings might end up on a individual 
        //class with notification settings
        public string Language { get; set; } = "pt-pt";
        public string Theme { get; set; } = "light";
    }
}
