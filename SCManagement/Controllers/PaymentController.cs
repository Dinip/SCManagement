﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.PaymentService;
using Microsoft.AspNetCore.Identity;
using SCManagement.Models;
using System.Text.RegularExpressions;

namespace SCManagement.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Payments controller constructor, injects all the services needed
        /// </summary>
        /// <param name="paymentService"></param>
        /// <param name="userManager"></param>
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

        /// <summary>
        /// Method to receive the webhook from easypay with the payment/subcription information
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/Payment/WebhookGeneric")]
        public async Task<IActionResult> WebhookGeneric([FromBody] PaymentWebhookGeneric notification)
        {
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


        /// <summary>
        /// Current payments associated with the user
        /// </summary>
        /// <param name="payId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string? payId)
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.PayId = payId;
            return View(await _paymentService.GetPayments(getUserIdFromAuthedUser()));
        }


        /// <summary>
        /// Pay a created payment with the selected payment method
        /// </summary>
        /// <param name="inputPayment"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaySinglePayment([Bind("Id,PaymentMethod,PhoneNumber")] PayPayment inputPayment)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index", new { payId = inputPayment.Id });
            var rg = new Regex("[0-9]{9}");
            if (inputPayment.PaymentMethod == PaymentMethod.MbWay && (inputPayment.PhoneNumber == null || !rg.IsMatch(inputPayment.PhoneNumber)))
            {
                TempData["Error"] = "MbWay requires phone number";
                return RedirectToAction("Index", new { payId = inputPayment.Id });
            }

            var payment = await _paymentService.PaySinglePayment(inputPayment);
            if (payment != null)
            {
                return RedirectToAction(nameof(Index), new { payId = payment.Id });
            }

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Shows all detailed information about a payment (partial view)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return PartialView("_CustomErrorPartial", "Error_NotFound");

            var payment = await _paymentService.GetPayment((int)id);
            if (payment == null || payment.UserId != getUserIdFromAuthedUser()) return PartialView("_CustomErrorPartial", "Error_NotFound");

            ViewBag.AutoRenew = false;

            if (payment.SubscriptionId != null)
            {
                var sub = await _paymentService.GetSubscription((int)payment.SubscriptionId);
                ViewBag.AutoRenew = sub?.AutoRenew ?? false;
                ViewBag.ClubName = sub?.Club?.Name ?? "";
            }

            ViewBag.PaymentMethods = from PaymentMethod pm in Enum.GetValues(typeof(PaymentMethod)) select new IdNameModel { Id = (int)pm, Name = pm.ToString() };

            return PartialView("_DetailsPartial", payment);
        }

        public class IdNameModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
