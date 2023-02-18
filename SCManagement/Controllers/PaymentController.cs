using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.PaymentService;
using Microsoft.AspNetCore.Identity;
using SCManagement.Models;
using Microsoft.AspNetCore.SignalR;

namespace SCManagement.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly UserManager<User> _userManager;

        public PaymentController(IPaymentService paymentService, UserManager<User> userManager)
        {
            _paymentService = paymentService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get id of the user that makes the request
        /// </summary>
        /// <returns>Returns the user id</returns>
        private string getUserIdFromAuthedUser()
        {
            //reminder: this does not query the db, it just gets the id from the token (claims)
            return _userManager.GetUserId(User);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/Payment/Webhook")]
        public async Task<IActionResult> Webhook([FromBody] PaymentWebhook notification)
        {
            //check if security code is present in the request, otherwise reject it
            string verifyCode = Request.Headers["x-easypay-code"];
            if (verifyCode != "admin123") return BadRequest(); //change to secret later

            //await _paymentService.UpdatePaymentFromWebhook(notification);
            //Console.WriteLine(verifyCode);
            //Console.WriteLine(notification);
            Console.WriteLine("Normal hook");
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/Payment/WebhookGeneric")]
        public async Task<IActionResult> WebhookGeneric([FromBody] PaymentWebhookGeneric notification)
        {
            //check if security code is present in the request, otherwise reject it
            string verifyCode = Request.Headers["x-easypay-code"];
            Console.WriteLine(verifyCode);
            Console.WriteLine(notification);

            if (verifyCode != "admin123") return BadRequest(); //change to secret later


            if (notification.type == "capture")
            {
                await _paymentService.WebhookHandleSinglePayment(notification);
            }
            else if (notification.type == "subscription_create")
            {
                await _paymentService.WebhookHandleSubscriptionCreate(notification);
            }
            else if (notification.type == "subscription_capture")
            {
                await _paymentService.WebhookHandleSubscriptionPayment(notification);
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }

        public async Task<IActionResult> Index(string? payId)
        {
            ViewBag.PayId = payId;
            return View(await _paymentService.GetPayments(getUserIdFromAuthedUser()));
        }

        //public async Task<IActionResult> Edit([Bind("Id,Name,Email,PhoneNumber,About,CreationDate,File,RemoveImage,ModalitiesIds")] EditModel club)

        public async Task<IActionResult> CreatePayment(int? productId)
        {
            //check if has product id in query and if exists
            if (productId == null) return View("CustomError", "Error_No_Product");
            var product = await _paymentService.GetProduct((int)productId);
            if (product == null) return View("CustomError", "Error_NotFound");

            //payment methods to viewbag with name and id
            ViewBag.PaymentMethods = from PaymentMethod pm in Enum.GetValues(typeof(PaymentMethod)) select new IdNameModel { Id = (int)pm, Name = pm.ToString() };

            return View(new CreatePayment { ProductId = product.Id, Product = product });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayment([Bind("ProductId,PaymentMethod,PhoneNumber,AutoRenew")] CreatePayment payment)
        {

            if (!ModelState.IsValid) return RedirectToAction("CreatePayment", new { productId = payment.ProductId });

            var userId = getUserIdFromAuthedUser();
            Payment? paymentResponse;

            if (payment.AutoRenew)
            {
                paymentResponse = await _paymentService.CreateSubscriptionPayment(payment, userId);
            }
            else
            {
                paymentResponse = await _paymentService.CreatePayment(payment, userId);
            }


            //view de resultado de pagamento, em espera se for mbway, com referencia se for MB ou
            //abrir nova janela se for cartao??
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PaymentStatus(int? id)
        {
            if (id == null) return View("CustomError", "Error_No_Payment");

            var payment = await _paymentService.GetPayment((int)id);
            if (payment == null || payment.UserId != getUserIdFromAuthedUser()) return View("CustomError", "Error_NotFound");

            return View(payment);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return PartialView("_CustomErrorPartial", "Error_NotFound");

            var payment = await _paymentService.GetPayment((int)id);
            if (payment == null || payment.UserId != getUserIdFromAuthedUser()) return PartialView("_CustomErrorPartial", "Error_NotFound");

            return PartialView("_DetailsPartial", payment);
        }

        public class IdNameModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
