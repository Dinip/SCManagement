using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Services.StatisticsService.Models
{
    [NotMapped]
    public class ClubCurrentUsers
    {
        public string RoleName { get; set; }
        public int Value { get; set; }
        public int? MaxValue { get; set; }
    }
}
