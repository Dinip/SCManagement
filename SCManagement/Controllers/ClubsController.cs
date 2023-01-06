using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.UserService;

namespace SCManagement.Controllers
{

    /// <summary>
    /// This class represents the Clubs Controller
    /// </summary>
    /// 
    public class ClubsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAzureStorage _azureStorage;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;

        /// <summary>
        /// This is the constructor of the Clubs Controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="azureStorage"></param>
        /// <param name="clubService"></param>
        public ClubsController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IAzureStorage azureStorage,
            IClubService clubService,
            IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _azureStorage = azureStorage;
            _clubService = clubService;
            _userService = userService;
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

            //If the club does not have an image, one is placed by default.
            if (club.Photography == null)
            {
                club.Photography = new BlobDto { Uri = "https://cdn.scmanagement.me/public/user_placeholder.png" };
            }

            ViewBag.NumberOfPartners = 5;

            string userId = getUserIdFromAuthedUser();

            //if the user has a role in the club, cant be partner
            if (_clubService.IsClubMember(userId, club.Id))
            {
                ViewBag.btnValue = "Become partner";
                ViewBag.btnClasses = "disabled";
            }
            //Is the user already a member? so stop being
            else if (_clubService.IsClubPartner(userId, club.Id))
            {
                ViewBag.btnValue = "Remove partner";
            }
            //the user is not yet a member, so he becomes one(user without roles in the club)
            else
            {
                ViewBag.btnValue = "Become partner";
            }

            return View("Details", club);
        }


        /// <summary>
        /// This method returns the Create View
        /// </summary>
        /// <returns>Create View</returns>
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Modalities = new SelectList(_clubService.GetModalities().Result, "Id", "Name");

            Address address = new Address();

            Club club = new Club
            {
                Address = address
            };

            return View(club);
        }


        /// <summary>
        /// This method returns the Create View
        /// </summary>
        /// <param name="club">Clube to be created</param>
        /// <returns>Index View</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ModalitiesIds")] Club club, int CountyId, string Street, string ZipCode, string Number)
        {
            if (!ModelState.IsValid) return View();

            //get the address
            int addressId = await _clubService.GetAddressAsync(CountyId, Street, ZipCode, Number);

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            Club createdClub = await _clubService.CreateClub(club, userId, addressId);

            await _userService.UpdateSelectedRole(userId, createdClub.UsersRoleClub!.First().Id);

            return RedirectToAction("Index", "MyClub");
        }

        /// <summary>
        /// This method allows adding a partner to the club if he has no role in the club, or allows removing a partner from the club if he is.
        /// </summary>
        /// <param name="id">id of the club</param>
        /// <returns>Deatils View</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Associate(int? id)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            string userId = getUserIdFromAuthedUser();
            Club? club = await _clubService.GetClub((int)id);
            if (club == null) return View("CustomError", "Error_NotFound");

            if (_clubService.IsClubPartner(userId, club.Id))
            {
                //remove a user from a club
                await _clubService.RemoveClubUser(userId, club.Id, 10);
            }
            else if (!_clubService.IsClubMember(userId, club.Id))
            {
                //add a associate to the club
                await _clubService.AddUserToClub(userId, club.Id, 10);
            }

            return RedirectToAction("Index", new { id });
        }

        [Authorize]
        public IActionResult Join(string? code)
        {
            return View(new CodeClub { Code = code });
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Join([Bind("Code")] CodeClub cc)
        {
            string userId = getUserIdFromAuthedUser();

            KeyValuePair<bool, string> joined = _clubService.UseCode(userId, cc);

            ViewBag.Message = joined.Value;
            if (joined.Key == false) return View("Join", new CodeClub { Code = cc.Code });

            return RedirectToAction("Index", "MyClub");
        }
    }
}
