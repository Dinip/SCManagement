using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SCManagement.Models;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.TeamService;
using SCManagement.Services.UserService;
using SCManagement.Services.Location;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SCManagement.Controllers
{

    /// <summary>
    /// This class represents the MyClub Controller
    /// </summary>
    /// 
    [Authorize]
    public class MyClubController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly ILocationService _locationService;


        /// <summary>
        /// This is the constructor of the MyClub Controller
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="clubService"></param>
        /// <param name="userService"></param>
        /// <param name="teamService"></param>
        public MyClubController(
            UserManager<User> userManager,
            IClubService clubService,
            IUserService userService,
            ITeamService teamService,
            ILocationService locationService)
        {
            _userManager = userManager;
            _clubService = clubService;
            _userService = userService;
            _teamService = teamService;
            _locationService = locationService;
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
        /// Gets the user selected club (if any)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            if (role.ClubId == 0) return View();

            ViewBag.RoleId = role.RoleId;

            return View(await _clubService.GetClub(role.ClubId));
        }

        /// <summary>
        /// This method returns the Edit View
        /// </summary>
        /// <param name="id">club id to be edited</param>
        /// <returns>Edit View</returns>
        public async Task<IActionResult> Edit()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //checl role
            if (!_clubService.IsClubAdmin(role)) return View("CustomError", "Error_Unauthorized");

            //get the club
            var club = await _clubService.GetClub(role.ClubId);

            //get ids of the modalities of the club
            List<int> ClubModalitiesIds = club.Modalities!.Select(m => m.Id).ToList();

            //viewbag that have the modalities of the club
            ViewBag.Modalities = new MultiSelectList(await _clubService.GetModalities(), "Id", "Name", ClubModalitiesIds);

            if (club == null) return View("CustomError", "Error_NotFound");

            var c = new EditModel
            {
                Id = club.Id,
                Name = club.Name,
                Email = club.Email,
                PhoneNumber = club.PhoneNumber,
                About = club.About,
                CreationDate = club.CreationDate,
                //AddressId = club.AddressId,
                //Address = club.Address,
                ModalitiesIds = ClubModalitiesIds,
                PhotographyId = club.PhotographyId,
                Photography = club.Photography,
                PhotoUri = club.Photography == null ? "https://cdn.scmanagement.me/public/user_placeholder.png" : club.Photography.Uri,
            };

            return View(c);
        }


        /// <summary>
        /// This method returns the Edit View
        /// </summary>
        /// <param name="id">id the clube to be edited</param>
        /// <param name="club">club to be edited</param>
        /// <returns>View Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name,Email,PhoneNumber,About,CreationDate,File,RemoveImage,ModalitiesIds")] EditModel club)
        {
            //check model state
            if (!ModelState.IsValid) return View(club);

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check roles
            if (!_clubService.IsClubAdmin(role)) return View("CustomError", "Error_Unauthorized");

            //check if the club being edited is the same
            //as the one the user has selected
            if (role.ClubId != club.Id) return View("CustomError", "Error_Unauthorized");

            //get club infos
            var actualClub = await _clubService.GetClub(role.ClubId);
            if (actualClub == null) return View("CustomError", "Error_NotFound");

            await _clubService.UpdateClubModalities(actualClub, club.ModalitiesIds!);

            //updates to the settings of club
            actualClub.Name = club.Name;
            actualClub.Email = club.Email;
            actualClub.PhoneNumber = club.PhoneNumber;
            actualClub.About = club.About;

            await _clubService.UpdateClubPhoto(actualClub, club.RemoveImage, club.File);

            //_clubService.UpdateClubAddress((int)actualClub.AddressId, CountyId, Street, ZipCode, Number);

            await _clubService.UpdateClub(actualClub);

            return RedirectToAction(nameof(Index));
        }

        public class EditModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Error_Required")]
            [StringLength(60, ErrorMessage = "Error_Length", MinimumLength = 2)]
            [Display(Name = "Club Name")]
            public string Name { get; set; }

            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }

            [Display(Name = "About Us")]
            public string? About { get; set; }

            public DateTime CreationDate { get; set; }

            public int? PhotographyId { get; set; }

            public BlobDto? Photography { get; set; }

            //public int? AddressId { get; set; }

            //public Address? Address { get; set; }

            [Display(Name = "Modalities")]
            [Required(ErrorMessage = "Error_Required")]
            public IEnumerable<int> ModalitiesIds { get; set; }

            public string? PhotoUri { get; set; }
            public IFormFile? File { get; set; }
            public bool RemoveImage { get; set; } = false;
        }

        /// <summary>
        /// Return a view which corresponds to the page that has the list of club members
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns>view PartenersList</returns>
        public async Task<IActionResult> PartnersList()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check roles
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.UserRoleId = role.RoleId;

            return View(await _clubService.GetClubPartners(role.ClubId));
        }

        /// <summary>
        /// Gets the staff list of the club based on the 
        /// selected club infered by the requester user
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> StaffList()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check roles
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.UserRoleId = role.RoleId;

            return View(await _clubService.GetClubStaff(role.ClubId));
        }

        /// <summary>
        /// Gets the athlete list of the club based on the 
        /// selected club infered by the requester user
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AthletesList()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check role
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.UserIsManager = _clubService.IsClubManager(role);

            return View(await _clubService.GetClubAthletes(role.ClubId));
        }

        /// <summary>
        /// Removes a user from the club
        /// </summary>
        /// <param name="usersRoleClubId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(int? usersRoleClubId, string? page)
        {
            if (usersRoleClubId == null) return View("CustomError", "Error_NotFound");

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            UsersRoleClub? userRoleToBeRomoved = await _clubService.GetUserRoleClubFromId((int)usersRoleClubId);

            //role with specified id does not exist or is club admin (which cant be removed)
            if (userRoleToBeRomoved == null || userRoleToBeRomoved.RoleId == 50) return View("CustomError", "Error_Unauthorized");

            //if the user role to be removed does not belong to the club of the user
            //that is making the request, then return unauthorized
            if (userRoleToBeRomoved.ClubId != role.ClubId) return View("CustomError", "Error_Unauthorized");

            //prevent users that arent club admin from removing club secretary (or higher)
            if (userRoleToBeRomoved.RoleId >= 40 && role.RoleId < 50) return View("CustomError", "Error_NotFound");

            //remove a user(role) from a club
            await _clubService.RemoveClubUser(userRoleToBeRomoved.Id);

            return RedirectToAction(page);
        }

        /// <summary>
        /// Creates a code to access the club (view)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreateCode()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.Roles = new SelectList(await _clubService.GetRoles(), "Id", "RoleName");

            return PartialView("_PartialCreateCode");
        }

        /// <summary>
        /// Creates a code to access the club (post request)
        /// </summary>
        /// <param name="codeClub"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCode([Bind("RoleId,ExpireDate")] CreateCodeModel codeClub)
        {
            if (!ModelState.IsValid) return RedirectToAction("Codes");

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");


            //generate a code
            DateTime? expireDate = codeClub.ExpireDate;
            if (expireDate != null)
            {
                expireDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            CodeClub codeToBeCreated = new CodeClub
            {
                ClubId = role.ClubId,
                CreatedByUserId = userId,
                RoleId = codeClub.RoleId,
                ExpireDate = expireDate,
                Approved = !(_clubService.IsClubSecretary(role) && codeClub.RoleId >= 40)
            };
            CodeClub generatedCode = await _clubService.GenerateCode(codeToBeCreated);

            return RedirectToAction("Codes", new { code = generatedCode.Id });
        }

        public class CreateCodeModel
        {
            [Required(ErrorMessage = "Error_Required")]
            [Display(Name = "Role")]
            public int RoleId { get; set; }

            [Display(Name = "Expire Date")]
            [DataType(DataType.Date)]
            public DateTime? ExpireDate { get; set; }
        }

        /// <summary>
        /// Gets the list of codes for the club with status
        /// </summary>
        /// <param name="approveCode"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IActionResult> Codes(string? approveCode, int? code)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.isAdmin = _clubService.IsClubAdmin(role);

            if (approveCode != null && _clubService.IsClubAdmin(role))
            {
                //approve the usage of the code
                ViewBag.ApprovedCodeStatus = _clubService.ApproveCode(approveCode);
                ViewBag.ApprovedCode = approveCode;
            }

            if (code != null)
            {
                CodeClub? cc = await _clubService.GetCodeWithInfos((int)code);
                if (cc != null && cc.ClubId == role.ClubId)
                {
                    ViewBag.Code = cc;
                }
            }

            return View(await _clubService.GetCodes(role.ClubId));
        }

        /// <summary>
        /// Sends an email with the code to access the club
        /// </summary>
        /// <param name="codeId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SendCodeEmail(int codeId, string email)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            await _clubService.SendCodeEmail(codeId, email, role.ClubId);

            return RedirectToAction("Codes", new { id = role.ClubId });
        }

        /// <summary>
        /// Gets the list os teams in a club
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> TeamList()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check role
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.IsManager = _clubService.IsClubManager(role);

            return View(await _teamService.GetTeams(role.ClubId));
        }

        /// <summary>
        /// Create a new team (view)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreateTeam()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check role Trainer
            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.Modalities = new SelectList(_clubService.GetClub(role.ClubId).Result.Modalities, "Id", "Name");

            return View();
        }

        /// <summary>
        /// Create a new team (post)
        /// </summary>
        /// <param name="team">Clube to be created</param>
        /// <returns>Index View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeam([Bind("Id,Name,ModalityId")] TeamModel team)
        {
            if (!ModelState.IsValid) return View(team);

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check role Trainer
            if (!_clubService.IsClubTrainer(role)) return View("CustomError", "Error_Unauthorized");

            await _teamService.CreateTeam(new Team { Name = team.Name, ModalityId = team.ModalityId, TrainerId = userId, ClubId = role.ClubId });

            return RedirectToAction(nameof(TeamList));

        }

        public class TeamModel
        {
            [Required(ErrorMessage = "Error_Required")]
            [StringLength(40, ErrorMessage = "Error_Length", MinimumLength = 2)]
            [Display(Name = "Team Name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Error_Required")]
            [Display(Name = "Modality")]
            public int ModalityId { get; set; }
        }


        /// <summary>
        /// Edit team view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditTeam(int? id)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check role
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            //get the club
            var club = await _clubService.GetClub(role.ClubId);

            //viewbag that have the modalities of the club
            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(club.Id), "Id", "Name");

            if (club == null) return View("CustomError", "Error_NotFound");

            //get the team
            var team = await _teamService.GetTeam((int)id);

            if (team == null) return View("CustomError", "Error_NotFound");

            return View(team);
        }

        /// <summary>
        /// Saves edited team
        /// </summary>
        /// <param name="id"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeam(int? id, [Bind("Id,Name,ModalityId")] TeamModel team)
        {
            //check model state
            if (!ModelState.IsValid) return View(team);

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check roles
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            //Update Team Atributes
            Team teamToUpdate = await _teamService.GetTeam((int)id);
            if (teamToUpdate == null) return View("CustomError", "Error_NotFound");

            //Check if team have modification
            if (teamToUpdate.Name == team.Name && teamToUpdate.ModalityId == team.ModalityId) return RedirectToAction(nameof(TeamList));


            teamToUpdate.Name = team.Name;
            teamToUpdate.ModalityId = team.ModalityId;

            //Update Team On DataBase
            await _teamService.UpdateTeam(teamToUpdate);

            return RedirectToAction(nameof(TeamList));
        }

        /// <summary>
        /// Gets the partial view with the list of athletes available
        /// to be added to a team
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddTeamAthletes(int id)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            Team? team = await _teamService.GetTeam(id);

            if (team == null) return View("CustomError", "Error_NotFound");

            //if he is trainer need to be the trainer of the team
            if (_clubService.IsClubTrainer(role) && team.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            List<User> clubAthletes = (await _clubService.GetAthletes(team.ClubId)).ToList();
            //check which athletes are available 
            IEnumerable<User> avaliableAthletes = clubAthletes.Where(x => !team.Athletes.Any(y => y.Id == x.Id)).ToList();


            return PartialView("_PartialAddTeamAthletes", avaliableAthletes);
        }

        /// <summary>
        /// Adds an athlete to a team (post)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selectedAthletes"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeamAthletes(int id, List<string> selectedAthletes)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check user role
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            Team? team = await _teamService.GetTeam(id);

            if (team == null) return View("CustomError", "Error_NotFound");

            //if he is trainer need to be the trainer of the team
            if (_clubService.IsClubTrainer(role) && team.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            await _teamService.UpdateTeamAthletes(id, selectedAthletes);

            return RedirectToAction(nameof(EditTeam), new { id = team.Id });
        }

        /// <summary>
        /// Remove an athlete from a team
        /// </summary>
        /// <param name="athleteId"></param>
        /// <param name="teamId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAtheleFromTeam(string? athleteId, int? teamId, string? page)
        {
            if (athleteId == null) return View("CustomError", "Error_NotFound");
            if (teamId == null) return View("CustomError", "Error_NotFound");

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);


            //check user role
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            User athleteToRemove = await _userService.GetUser(athleteId);
            //Chekc if athlete exists
            if (athleteToRemove == null) return View("CustomError", "Error_NotFound");

            //check if athlete is on team
            Team team = await _teamService.GetTeam((int)teamId);

            //check if are using this service its good ??
            if (!team.Athletes.Contains(athleteToRemove)) return View("CustomError", "Error_NotFound");

            await _teamService.RemoveAthlete(team, athleteToRemove);

            return RedirectToAction(nameof(EditTeam), new { id = team.Id });
        }

        /// <summary>
        /// Gets the team details and athletes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> TeamDetails(int? id)
        {
            Team team = await _teamService.GetTeam((int)id);
            if (team == null) return View("CustomError", "Error_NotFound");

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //Check if is member of the club
            if (!_clubService.IsClubMember(role.UserId, role.ClubId)) return View("CustomError", "Error_Unauthorized");

            return View(team);
        }

        /// <summary>
        /// Deletes a team
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeam(int? id)
        {
            Team team = await _teamService.GetTeam((int)id);
            if (team == null) return View("CustomError", "Error_NotFound");

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //Check if is staff
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            //Check if is trainer
            if (_clubService.IsClubTrainer(role) && team.TrainerId != userId) return View("CustomError", "Error_Unauthorized");

            await _teamService.DeleteTeam(team);

            return RedirectToAction(nameof(TeamList));
        }

        /// <summary>
        /// Gets the teams that an athlete is in
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> MyTeams()
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //Check if is athlete
            if (!_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            var teams = await _teamService.GetTeamsByAthlete(userId, role.ClubId);

            return View(teams);
        }

        /// <summary>
        /// Gets the user details in a partial view to be used
        /// in a modal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null) return PartialView("_CustomErrorPartial", "Error_NotFound");

            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //Check if is staff
            if (!_clubService.IsClubMember(userId, role.ClubId)) return PartialView("_CustomErrorPartial", "Error_Unauthorized");

            //check if the athlete belongs to the club
            if (!_clubService.IsClubMember(id, role.ClubId)) return PartialView("_CustomErrorPartial", "Error_Unauthorized");

            var user = await _userService.GetUser(id);
            if (user == null) return PartialView("_CustomErrorPartial", "Error_NotFound");

            var userRoleId = _clubService.GetUserRoleInClub(user.Id, role.ClubId);
            if (userRoleId > role.RoleId) return PartialView("_CustomErrorPartial", "Error_Unauthorized");

            return PartialView("_PartialUserDetails", user); ;
        }

        public IActionResult NewAdress()
        {

            return View();
        }

        /// <summary>
        /// Allow to create a address to the club or update
        /// </summary>
        /// <param name="CoordinateX"></param>
        /// <param name="CoordinateY"></param>
        /// <param name="ZipCode"></param>
        /// <param name="Street"></param>
        /// <param name="City"></param>
        /// <param name="District"></param>
        /// <param name="Country"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReceveAddress(double CoordinateX, double CoordinateY, string ZipCode, string Street, string City, string District, string Country)
        {
            //get id of the user
            string userId = getUserIdFromAuthedUser();

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            //check if the user is a admin of the club
            if (!_clubService.IsClubAdmin(role)) return View("CustomError", "Error_Unauthorized");

            //get the club
            var club = await _clubService.GetClub(role.ClubId);

            if (club == null) return NotFound();

            var clubAddresId = club.AddressId;

            if (clubAddresId != null) 
            {
                // update Address
                _clubService.UpdateClubAddress(CoordinateX, CoordinateY, ZipCode, Street, City, District, Country, (int)clubAddresId);
                return Json(clubAddresId);
            } 
            
            //create address
            var resCreate = await _clubService.CreateAddress(CoordinateX, CoordinateY, ZipCode, Street, City, District, Country, club.Id);
            return Json(resCreate);

        }


    }
}
