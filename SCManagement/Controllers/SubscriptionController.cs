﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.PaymentService;
using Microsoft.AspNetCore.Identity;
using SCManagement.Models;

namespace SCManagement.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly UserManager<User> _userManager;

        public SubscriptionController(IPaymentService paymentService, UserManager<User> userManager)
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

        public async Task<IActionResult> Index(string? subId)
        {
            ViewBag.SubId = subId;
            return View(await _paymentService.GetSubscriptions(getUserIdFromAuthedUser()));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return PartialView("_CustomErrorPartial", "Error_NotFound");

            var sub = await _paymentService.GetSubscription((int)id);
            if (sub == null || sub.UserId != getUserIdFromAuthedUser()) return PartialView("_CustomErrorPartial", "Error_NotFound");

            return PartialView("_DetailsPartial", sub);
        }

        public async Task<IActionResult> UpdateAutoRenew(int? id)
        {
            if (id == null) return PartialView();

            var sub = await _paymentService.GetSubscription((int)id);
            if (sub == null) return PartialView();

            if (sub.AutoRenew)
            {
                await _paymentService.CancelAutoSubscription(sub.Id);
            }
            else
            {
                await _paymentService.SetSubscriptionToAuto(sub.Id);
            }

            return RedirectToAction("Index", new { subId = sub.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int? id)
        {
            var sub = await _paymentService.GetSubscription((int)id);
            if (sub == null || sub.UserId != getUserIdFromAuthedUser()) return Json(new { status = "not ok" });

            await _paymentService.CancelSubscription(sub.Id);

            return Json(new { status = "ok" });
        }
    }
}
