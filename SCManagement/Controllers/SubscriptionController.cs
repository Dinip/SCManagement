using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.PaymentService;
using Microsoft.AspNetCore.Identity;
using SCManagement.Models;
using SCManagement.Services.ClubService;

namespace SCManagement.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IClubService _clubService;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Subscription controller constructor, injects all the services needed
        /// </summary>
        /// <param name="paymentService"></param>
        /// <param name="userManager"></param>
        /// <param name="clubService"></param>
        public SubscriptionController(IPaymentService paymentService, UserManager<User> userManager, IClubService clubService)
        {
            _paymentService = paymentService;
            _userManager = userManager;
            _clubService = clubService;
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
        /// Current subscriptions associated with the user (includes expired and canceled)
        /// </summary>
        /// <param name="subId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string? subId)
        {
            ViewBag.SubId = subId;
            ViewBag.InnerError = TempData["InnerError"];
            ViewBag.Error = TempData["Error"];
            ViewBag.Message = TempData["Message"];
            return View(await _paymentService.GetSubscriptions(getUserIdFromAuthedUser()));
        }


        /// <summary>
        /// Shows all detailed information about a subscription (partial view)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return PartialView("_CustomErrorPartial", "Error_NotFound");

            var sub = await _paymentService.GetSubscription((int)id);
            if (sub == null || sub.UserId != getUserIdFromAuthedUser()) return PartialView("_CustomErrorPartial", "Error_NotFound");

            return PartialView("_DetailsPartial", sub);
        }


        /// <summary>
        /// A toggle method to enable or disable auto renew for a subscription
        /// Uses last state as base and inverts it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAutoRenew(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Error_NotFound_Title";
                return RedirectToAction(nameof(Index));
            }

            var sub = await _paymentService.GetSubscription((int)id);
            if (sub == null)
            {
                TempData["Error"] = "Error_NotFound_Title";
                return RedirectToAction(nameof(Index));
            }

            if (sub.AutoRenew)
            {
                await _paymentService.CancelAutoSubscription(sub.Id);
                TempData["Message"] = "SubCancelAutoSuccess";
            }
            else
            {
                await _paymentService.SetSubscriptionToAuto(sub.Id);
                TempData["Message"] = "SubEnableAutoSuccess";
            }

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Cancels the subscription (at the end of the cicle)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Error_NotFound_Title";
                return RedirectToAction(nameof(Index));
            }

            var sub = await _paymentService.GetSubscription((int)id);
            if (sub == null || sub.UserId != getUserIdFromAuthedUser())
            {
                TempData["Error"] = "Error_NotFound_Title";
                return RedirectToAction(nameof(Index));
            }

            await _paymentService.CancelSubscription(sub.Id);
            TempData["Message"] = "SubCancelSuccess";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Upgrade the plan of a club plan subscription
        /// Validates that the plan can't be downgraded to
        /// another with less slots than the total athletes now
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpgradeClubPlan(UpgradePlan plan)
        {
            if (!ModelState.IsValid)
            {
                TempData["InnerError"] = "Null SubId or PlanId";
                return RedirectToAction(nameof(Index), new { subId = plan.SubscriptionId });
            };

            var sub = await _paymentService.GetSubscription(plan.SubscriptionId);
            if (sub == null || sub.UserId != getUserIdFromAuthedUser())
            {
                TempData["InnerError"] = "Invalid subscription";
                return RedirectToAction(nameof(Index), new { subId = plan.SubscriptionId });
            };

            try
            {
                await _paymentService.UpgradeClubPlan(plan.SubscriptionId, plan.PlanId);
            }
            catch (Exception ex)
            {
                TempData["InnerError"] = ex.Message;
                return RedirectToAction(nameof(Index), new { subId = plan.SubscriptionId });
            }

            return RedirectToAction(nameof(Index), new { subId = plan.SubscriptionId });
        }
    }
}
