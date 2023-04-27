﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SCManagement.Models;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PaymentService;
using SCManagement.Services.UserService;

namespace SCManagement.Controllers
{
    public class ClubsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// This is the constructor of the Clubs Controller
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="clubService"></param>
        /// <param name="userService"></param>
        /// <param name="paymentService"></param>
        public ClubsController(
            UserManager<User> userManager,
            IClubService clubService,
            IUserService userService,
            IPaymentService paymentService)
        {
            _userManager = userManager;
            _clubService = clubService;
            _userService = userService;
            _paymentService = paymentService;
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
        /// This method returns the Index View
        /// </summary>
        /// <returns>Index view </returns>
        public async Task<IActionResult> Index()
        {
            return View(await _clubService.GetClubs());
        }

        /// <summary>
        /// this method alow to send to mapbox coordinates of the clubs
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CoordsMarkers()
        {
            return Json(await _clubService.GetAllCoordinates());
        }

        /// <summary>
        /// this method allow to send the results of a search to a list of cards with clubs
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IActionResult> SearchNameClubs(string name)
        {
            var matchingClubs = await _clubService.SearchNameClubs(name);
            return PartialView("_PartialSearchClub", matchingClubs);
        }

        /// <summary>
        /// Force redirect from /Clubs/Details/id to /Clubs/id
        /// </summary>
        /// <param name="id">id of the club</param>
        /// <returns>redirect</returns>
        public IActionResult Details(int? id) => RedirectPermanent($"/Clubs/{id}");

        /// <summary>
        /// This method returns the Details View
        /// </summary>
        /// <param name="id">club id to view</param>
        /// <returns>Deatils view</returns>
        [Route("Clubs/{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            //get the club to be viewed
            var club = await _clubService.GetClub(id);

            if (club == null)
            {
                return View("CustomError", "Error_NotFound");
            }

            club.ClubTranslations = (ICollection<ClubTranslations>?)await _clubService.GetClubTranslations(id);

            //If the club does not have an image, one is placed by default.
            if (club.Photography == null)
            {
                club.Photography = new BlobDto { Uri = "https://cdn.scmanagement.me/public/user_placeholder.png" };
            }

            string userId = getUserIdFromAuthedUser();

            bool isPartner = _clubService.IsClubPartner(userId, club.Id);
            ViewBag.AlreadyPartner = isPartner;
            if (isPartner)
            {
                var sub = await _paymentService.GetMembershipSubscription(userId, club.Id);
                ViewBag.SubId = sub?.Id ?? 0;
            }
            ViewBag.IsClubMember = _clubService.IsClubMember(userId, club.Id);

            return View("Details", club);
        }


        /// <summary>
        /// This method returns the Create View
        /// </summary>
        /// <returns>Create View</returns>
        [Authorize]
        public async Task<IActionResult> Create(int? planId)
        {
            if (planId == null) return RedirectToAction(nameof(Plans));

            var plan = await _paymentService.GetClubSubscriptionPlan((int)planId);

            if (plan == null) return RedirectToAction(nameof(Plans));

            ViewBag.SelectedPlan = plan;
            ViewBag.Modalities = new SelectList(await _clubService.GetModalitiesToSelectList(), "Id", "Name");

            return View(new CreateClubModel());
        }

        public class CreateClubModel
        {
            [Required(ErrorMessage = "Error_Required")]
            [StringLength(60, ErrorMessage = "Error_Length", MinimumLength = 2)]
            [Display(Name = "Club Name")]
            public string Name { get; set; }

            [Display(Name = "Modalities")]
            [Required(ErrorMessage = "Error_Required")]
            public IEnumerable<int>? ModalitiesIds { get; set; }


            [Display(Name = "Plan")]
            [Required(ErrorMessage = "Error_Required")]
            public int PlanId { get; set; }
        }

        /// <summary>
        /// This method returns the Create View
        /// </summary>
        /// <param name="clubInput">Clube to be created</param>
        /// <returns>Index View</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ModalitiesIds,PlanId")] CreateClubModel clubInput)
        {
            ViewBag.Modalities = new SelectList(await _clubService.GetModalitiesToSelectList(), "Id", "Name");

            var clubSubProducts = await _paymentService.GetClubSubscriptionPlans();
            ViewBag.Plans = clubSubProducts;

            if (!ModelState.IsValid) return View(clubInput);

            var plan = await _paymentService.GetProduct(clubInput.PlanId);
            if (plan == null || plan.ProductType != Services.PaymentService.Models.ProductType.ClubSubscription) return View(clubInput);

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            Club c = new Club
            {
                Name = clubInput.Name,
                Modalities = (await _clubService.GetModalities()).Where(m => clubInput.ModalitiesIds.Contains(m.Id)).ToList(),
                Status = plan.Value > 0 ? ClubStatus.Waiting_Payment : ClubStatus.Active,
            };

            if (!clubSubProducts.Any(s => s.Id == clubInput.PlanId))
            {
                return View(clubInput);
            }

            Club createdClub = await _clubService.CreateClub(c, userId);

            await _paymentService.SubscribeClubToPlan(createdClub.Id, userId, clubInput.PlanId);

            await _paymentService.UpdateProductClubMembership(new ClubPaymentSettings
            {
                ClubPaymentSettingsId = createdClub.Id,
                QuotaFee = 0,
                QuotaFrequency = Services.PaymentService.Models.SubscriptionFrequency.Monthly
            });

            await _userService.UpdateSelectedRole(userId, createdClub.UsersRoleClub!.First().Id);

            return RedirectToAction("Index", "Subscription");
        }

        /// <summary>
        /// This method allows adding a partner to the club if he has no role in the club, or allows removing a partner from the club if he is.
        /// Be careful with this method, since it does not have the
        /// [Authorize] flag, but should only be called when authed.
        /// Instead, it checks if the user has id (which is only possible
        /// if he is authed), and if the value is null, it redirects to the
        /// login page with the Clubs/Id as the return url.
        /// </summary>
        /// <param name="Id">id of the club</param>
        /// <returns>Deatils View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Associate(int? Id)
        {
            if (Id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();
            // check user auth and if not authed, redirect to
            // login page with custom return url
            if (userId == null) return RedirectToPage("/Account/Login", new
            {
                area = "Identity",
                ReturnUrl = $"/Clubs/{Id}"
            });

            Club? club = await _clubService.GetClub((int)Id);
            if (club == null) return View("CustomError", "Error_NotFound");

            if (!_clubService.IsClubMember(userId, club.Id) || !_clubService.IsClubPartner(userId, club.Id))
            {
                //add a associate to the club
                var sub = await _paymentService.CreateMembershipSubscription(userId, club.Id);
                await _clubService.AddPartnerToClub(userId, club.Id, sub.Value > 0 ? UserRoleStatus.Pending_Payment : UserRoleStatus.Active);

                return RedirectToAction("Index", "Subscription", new { subId = sub.Id });
            }

            return RedirectToAction("Index", new { Id });
        }

        /// <summary>
        /// View to use the code to join a club
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult Join(string? code)
        {
            return View(new CodeClub { Code = code });
        }

        /// <summary>
        /// Join a club with a code (gives error if the code is invalid,
        /// already used, user already part of club or expired)
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Join([Bind("Code")] CodeClub cc)
        {
            string userId = getUserIdFromAuthedUser();

            KeyValuePair<bool, string> joined = await _clubService.UseCode(userId, cc);

            ViewBag.Message = joined.Value;
            if (joined.Key == false) return View("Join", new CodeClub { Code = cc.Code });
            return RedirectToAction("Index", "MyClub");
        }

        /// <summary>
        /// Gets all available plans to subscribe to create a club
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Plans()
        {
            return View(await _paymentService.GetClubSubscriptionPlans());
        }
    }
}
