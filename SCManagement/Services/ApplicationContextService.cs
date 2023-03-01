using SCManagement.Models;

namespace SCManagement.Services
{
    public class ApplicationContextService
    {
        public string UserId { get; set; }
        public UsersRoleClub UserRole { get; set; }
        public ClubStatus ClubStatus { get; set; }
    }
}
