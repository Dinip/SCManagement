namespace SCManagement.Models
{
    public class EventResult
    {
        public int Id { get; set; }
        public int? Position { get; set; }
        public double? Time { get; set; }
        public int? Score { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
    }


}
