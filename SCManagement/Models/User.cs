using Microsoft.AspNetCore.Identity;
using System.Net;

namespace SCManagement.Models
{
    public class User : IdentityUser
    {
        
        public string Name { get; set; }

        public string SurName { get; set; }
        
        public string ProfilePicture { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int UserSettingsId { get; set; }

        public UserSettings UserSettings { get; set; }

        public int AddressId { get; set; }

        public Address Address { get; set; }
    }
}
