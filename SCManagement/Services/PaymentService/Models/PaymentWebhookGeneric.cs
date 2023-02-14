namespace SCManagement.Services.PaymentService.Models
{
    public class PaymentWebhookGeneric
    {
        public string id { get; set; }
        public string key { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string[]? messages { get; set; }
        public string? date { get; set; }

        public override string ToString()
        {
            return $"id: {id}, key: {key}, type: {type}, status: {status}, date: {date}";
        }
    }
}

//generic
//{
//  "id": "1bbc14c3-8ca8-492c-887d-1ca86400e4fa",
//  "key": "Example Key",
//  "type": "capture",
//  "status": "success",
//  "messages": [
//    "Your request was successfully created"
//  ],
//  "date": "2022-01-01 10:20:30"
//}

//payment
//{
//    "id": "21e1dc2d-dabe-4e33-b759-3b0606b80037",
//    "value": "10",
//    "currency": "EUR",
//    "key": "",
//    "expiration_time": "",
//    "method": "MB",
//    "customer": {
//        "id": "3676fb75-b074-4201-9a90-51fc1b3eb94f",
//        "name": "",
//        "email": "",
//        "phone": "",
//        "phone_indicative": "",
//        "fiscal_number": "",
//        "key": ""
//    },
//    "account": {
//        "id": ""
//    },
//    "transaction": {
//        "id": "8a50161a-d26e-43f6-b7c2-99a0255ada23",
//        "key": "",
//        "type": "capture",
//        "date": "2023-02-05T21:21:21Z",
//        "transfer_date": "2023-02-08T00:00:00Z",
//        "document_number": "BUSINE0712220465385820230205212041",
//        "values": {
//            "requested": "10",
//            "paid": "10",
//            "fixed_fee": "0.25",
//            "variable_fee": "0.15",
//            "tax": "0",
//            "transfer": "9.508"
//        }
//    }
//}
