namespace SCManagement.Models
{
    public class EventTranslation : ITranslation
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public string? Language { get; set; }
        public string? Value { get; set; }
        public string? Atribute { get; set; }
    }
}
