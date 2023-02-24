namespace SCManagement.Services.PaymentService.Models
{
    public class EasypayConfigResponse
    {
        public string? generic { get; set; }
        public string? account { get; set; }
        public List<string> payment_methods { get; set; }
    }
}
