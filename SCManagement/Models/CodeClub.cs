using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SCManagement.Models
{
    [Index(nameof(Code), IsUnique = true)]
    public class CodeClub
    {
        public int Id { get; set; }

        [StringLength(10)]
        public string Code { get; set; }

        //little trick to make creation date automatically use the current date
        private DateTime? dateCreated = null;

        [DataType(DataType.Date)]
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
        public DateTime? ExpireDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? UsedDate { get; set; }
        
        public bool Approved { get; set; } = true;
        
        public int ClubId { get; set; }
        public Club? Club { get; set; }
        public int RoleId { get; set; }
        public RoleClub? Role { get; set; }
        
        public string CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
        public string? UsedByUserId { get; set; }
        public User? UsedByUser { get; set; }
    }
}
