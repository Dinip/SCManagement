﻿namespace SCManagement.Models {
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
        public DateTime? JoinDate { get; set; }
    }
}
