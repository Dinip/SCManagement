using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Services.ClubService.Models
{
    [NotMapped]
    public class ClubSlots
    {
        public int TotalSlots { get; set; } = 0;
        public int AvailableSlots { get; set; } = 0;
        public int UsedSlots { get; set; } = 0;
    }
}
