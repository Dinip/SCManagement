using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using SCManagement.Services.AzureStorageService.Models;
using Xunit.Abstractions;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace SCManagement.Models {
    /// <summary>
    /// This class represents a Club
    /// </summary>
    public class Club {

        public int Id { get; set; }

        [Required(ErrorMessage = "Error_Required")]
        [StringLength(60, ErrorMessage = "Error_Length", MinimumLength = 2)]
        [Display(Name = "Club Name")]
        public string Name { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "About Us")]
        public string? About { get; set; }

        [Display(Name = "Terms and conditions")]
        public string? TermsAndConditions { get; set; }

        //little trick to make creation date automatically use the current date
        public DateTime CreationDate
        {
            get
            {
                return dateCreated.HasValue
                   ? dateCreated.Value
                   : DateTime.Now;
            }

            set { dateCreated = value; }
        }

        private DateTime? dateCreated = null;

        public DateTime? EndDate { get; set; }

        public int? PhotographyId { get; set; }

        public BlobDto? Photography { get; set; }

        public int? AddressId { get; set; }

        public Address? Address { get; set; }

        public ICollection<Modality>? Modalities { get; set; }

        public ICollection<UsersRoleClub>? UsersRoleClub { get; set; }

        [NotMapped]
        [Display(Name = "Modalities")]
        [Required(ErrorMessage = "Error_Required")]
        public IEnumerable<int>? ModalitiesIds { get; set; }
    }
}
