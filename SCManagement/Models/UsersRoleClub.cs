using System.ComponentModel.DataAnnotations;

namespace SCManagement.Models {
    /// <summary>
    /// This class represents a User Role Club
    /// </summary>
    public class UsersRoleClub {
        public int Id { get; set; }

        public int ClubId { get; set; }
        [Display(Name = "Club")]
        public Club? Club { get; set; }

        public string UserId { get; set; }
        [Display(Name = "User")]
        public User? User { get; set; }

        public int RoleId { get; set; }
        [Display(Name = "Role")]
        public RoleClub? Role { get; set; }
        //little trick to make creation date automatically use the current date
        [Display(Name = "JoinDate")]
        public DateTime? JoinDate
        {
            get
            {
                return dateJoined.HasValue
                   ? dateJoined.Value
                   : DateTime.Now;
            }

            set { dateJoined = value; }
        }
        private DateTime? dateJoined = null;

        public bool Selected { get; set; } = false;
    }
}
