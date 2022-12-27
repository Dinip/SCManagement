namespace SCManagement.Models
{
    /// <summary>
    /// This class represents a Modality
    /// </summary>
    public class Modality
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Club>? Clubs { get; set; }
    }
}
