namespace SCManagement.Services.PaymentService.Models
{
    public class CardInfo
    {
        public string LastFourDigits { get; set; }
        public string Type { get; set; }
        public string ExpirationDate { get; set; }
    }
}
