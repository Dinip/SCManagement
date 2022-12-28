namespace SCManagement.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string? Number { get; set; }
        public string? ZipCode { get; set; }
        public int CountyId { get; set; }
        public County? County { get; set; }
    }
}
