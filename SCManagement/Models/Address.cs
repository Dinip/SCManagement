using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Models
{
    public class Address
    {
        public int Id { get; set; }
        public double? CoordinateX { get; set; }
        public double? CoordinateY { get; set; }
        public string? ZipCode { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Country { get; set; }
    }
}
