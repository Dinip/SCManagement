using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Models
{
    public class Address
    {
        public int Id { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public string AddressString { get; set; }
    }
}
