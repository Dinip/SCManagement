namespace SCManagement.Services.PaymentService.Models
{
    public class EasypayResponse
    {
        public string status { get; set; }
        public List<string> message { get; set; }
        public string id { get; set; }
        public Method method { get; set; }
        public string? payment_status { get; set; }
    }

    public class Method
    {
        public string type { get; set; }
        public string entity { get; set; }
        public string reference { get; set; }
        public string url { get; set; }
        public string last_four { get; set; }
        public string card_type { get; set; }
        public string expiration_date { get; set; }
        public string iban { get; set; }
        public string status { get; set; }
        public string alias { get; set; }
    }
}
