namespace SCManagement.Models
{
    public class EventEnroll
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public DateTime EnrollDate { get; set; }
        public EnrollPaymentStatus EnrollStatus { get; set; }

    }

    public enum EnrollPaymentStatus : int
    {
        Pending = 1,
        Valid = 2,
    }
}
