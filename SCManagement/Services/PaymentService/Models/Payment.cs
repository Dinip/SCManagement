using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SCManagement.Models;

namespace SCManagement.Services.PaymentService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string PaymentKey { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public float Value { get; set; }
        public string? PhoneNumber { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        //little trick to make creation date automatically use the current date
        private DateTime? dateCreated = null;
        public DateTime CreatedAt
        {
            get
            {
                return dateCreated.HasValue
                   ? dateCreated.Value
                   : DateTime.Now;
            }

            set { dateCreated = value; }
        }
        public DateTime? PayedAt { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string? MbEntity { get; set; }
        public string? MbReference { get; set; }
        public string? Url { get; set; }
        public int? SubscriptionId { get; set; }
        public string? CardInfoData { get; set; }
        [NotMapped]
        public CardInfo? CardInfo
        {
            get
            {
                return string.IsNullOrEmpty(CardInfoData) ? null : JsonConvert.DeserializeObject<CardInfo>(CardInfoData);
            }
        }

        public static PaymentStatus ConvertStatus(string status)
        {
            if (status == "success" || status == "paid") return PaymentStatus.Paid;
            if (status == "pending") return PaymentStatus.Pending;
            if (status == "failed") return PaymentStatus.Failed;
            if (status == "deleted" || status == "voided") return PaymentStatus.Canceled;
            return PaymentStatus.Error;
        }

        public static string ConvertMethod(PaymentMethod paymentMethod)
        {
            if (paymentMethod == PaymentMethod.MbWay) return "mbw";
            if (paymentMethod == PaymentMethod.Reference) return "mb";
            return "cc";
        }
    }

    public enum PaymentStatus : int
    {
        Pending = 1,
        Paid = 2,
        Failed = 3,
        Error = 4,
        Canceled = 5
    }

    public enum PaymentMethod : int
    {
        Reference = 1,
        MbWay = 2,
        CreditCard = 3
    }
}
