using System.Diagnostics.Metrics;

namespace SCManagement.Models
{
    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public ICollection<County> Counties { get; set; }
    }
}
