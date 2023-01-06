namespace SCManagement.Models {
    /// <summary>
    /// This class represents a User Role Club
    /// </summary>
    public class UsersRoleClub {
        public int Id { get; set; }

        public int ClubId { get; set; }
        public Club? Club { get; set; }

        public string UserId { get; set; }
        public User? User { get; set; }

        public int RoleId { get; set; }
        public RoleClub? Role { get; set; }
        //little trick to make creation date automatically use the current date
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
