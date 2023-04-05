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

        public async Task<IActionResult> Index(string? subId)
        {
            ViewBag.SubId = subId;
            ViewBag.InnerError = TempData["InnerError"];
            ViewBag.Error = TempData["Error"];
            ViewBag.Message = TempData["Message"];
            return View(await _paymentService.GetSubscriptions(getUserIdFromAuthedUser()));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return PartialView("_CustomErrorPartial", "Error_NotFound");

            var sub = await _paymentService.GetSubscription((int)id);
            if (sub == null || sub.UserId != getUserIdFromAuthedUser()) return PartialView("_CustomErrorPartial", "Error_NotFound");

            return PartialView("_DetailsPartial", sub);
        }

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

        public async Task<IActionResult> PlansPartial(int id)
        {
            var sub = await _paymentService.GetSubscription(id);
            if (sub == null || sub.UserId != getUserIdFromAuthedUser()) PartialView("_CustomErrorPartial", "Error_NotFound");

            var athletes = (await _clubService.GetAthletes((int)sub.ClubId)).Count();
            var plans = await _paymentService.GetClubSubscriptionPlans();

            foreach (var plan in plans)
            {
                plan.Enabled = (plan.AthleteSlots < athletes || plan.Id == sub.ProductId);
            }

            return PartialView("_PlansPartial", new UpgradePlan
            {
                SubscriptionId = sub.Id,
                PlanId = sub.ProductId,
                Athletes = athletes,
                Plans = plans.ToList()
            }); ;
        }
    }
}
