using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SCManagement.Models
{
    [Index(nameof(Code), IsUnique = true)]
    public class CodeClub
    {
        public int Id { get; set; }

        [StringLength(10)]
        [Display(Name = "Code")]
        public string Code { get; set; }

        //little trick to make creation date automatically use the current date
        private DateTime? dateCreated = null;

        [DataType(DataType.Date)]
        [Display(Name = "Date Created")]
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
        [DataType(DataType.Date)]
        [Display(Name = "Date Expired")]
        public DateTime? ExpireDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date Used")]
        public DateTime? UsedDate { get; set; }

        [Display(Name = "Approved")]
        public bool Approved { get; set; } = true;
        
        public int ClubId { get; set; }
        [Display(Name = "Club")]
        public Club? Club { get; set; }
        public int RoleId { get; set; }
        [Display(Name = "Role")]
        public RoleClub? Role { get; set; }
        
        public string CreatedByUserId { get; set; }
        [Display(Name = "CreatedByUser")]
        public User? CreatedByUser { get; set; }
        public string? UsedByUserId { get; set; }
        [Display(Name = "UsedByUser")]
        public User? UsedByUser { get; set; }
    }
}
