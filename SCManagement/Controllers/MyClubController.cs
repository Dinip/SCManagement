using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Packaging;
using SCManagement.Models;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.TeamService;
using SCManagement.Services.TranslationService;
using SCManagement.Services.UserService;
using SCManagement.Services.PaymentService;
using SCManagement.Services;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.PlansService;
using SCManagement.Services.PlansService.Models;
using FluentEmail.Core;
using SCManagement.Data.Migrations;
using SCManagement.Services.NotificationService;

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
        private readonly ITranslationService _translationService;
        private readonly IPaymentService _paymentService;
        private readonly IPlanService _planService;
        private readonly ApplicationContextService _applicationContextService;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly IAzureStorage _azureStorage;
        

        /// <summary>
        /// This is the constructor of the MyClub Controller
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="clubService"></param>
        /// <param name="userService"></param>
        /// <param name="teamService"></param>
        /// <param name="translationService"></param>
        /// <param name="paymentService"></param>
        /// <param name="applicationContextService"></param>
        public MyClubController(
            UserManager<User> userManager,
            IClubService clubService,
            IUserService userService,
            ITeamService teamService,
            ITranslationService translationService,
            IPaymentService paymentService,
            ApplicationContextService applicationContextService,
            IStringLocalizer<SharedResource> stringLocalizer,
            IAzureStorage azureStorage,
            IPlanService planService
            )
        {
            _userManager = userManager;
            _clubService = clubService;
            _userService = userService;
            _teamService = teamService;
            _translationService = translationService;
            _paymentService = paymentService;
            _applicationContextService = applicationContextService;
            _stringLocalizer = stringLocalizer;
            _azureStorage = azureStorage;
            _planService = planService;
        }

        public async Task<IActionResult> Unavailable()
        {
            //get id of the user
            string userId = _userManager.GetUserId(User);

            //get the user selected role
            var role = await _userService.GetSelectedRole(userId);

            if (role.ClubId == 0) return RedirectToAction(nameof(Index));

            var status = await _clubService.GetClubStatus(role.ClubId);

            if (status == ClubStatus.Active) return RedirectToAction(nameof(Index));

            ViewBag.RoleId = role.RoleId;

            return View("Unavailable", await _clubService.GetClub(role.ClubId));
        }

        /// <summary>
        /// Gets the user selected club (if any)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            UsersRoleClub role = _applicationContextService.UserRole;

            if (role.RoleId == 0) return View();

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
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //checl role
            if (!_clubService.IsClubAdmin(role)) return View("CustomError", "Error_Unauthorized");

            //get the club
            var club = await _clubService.GetClub(role.ClubId);

            if (club == null) return View("CustomError", "Error_NotFound");

            //get ids of the modalities of the club
            List<int> ClubModalitiesIds = club.Modalities!.Select(m => m.Id).ToList();

            //viewbag that have the modalities of the club
            ViewBag.Modalities = new MultiSelectList(await _clubService.GetModalitiesToSelectList(), "Id", "Name", ClubModalitiesIds);

            ViewBag.CultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            ViewBag.Languages = new List<CultureInfo> { new("en-US"), new("pt-PT") };


            ICollection<ClubTranslations> About = club.ClubTranslations.Where(c => c.Atribute == "About").ToList();
            ICollection<ClubTranslations> Terms = club.ClubTranslations.Where(c => c.Atribute == "TermsAndConditions").ToList();

            var c = new EditModel
            {
                Id = club.Id,
                Name = club.Name,
                Email = club.Email,
                PhoneNumber = club.PhoneNumber,
                ClubTranslationsAbout = About,
                ClubTranslationsTerms = Terms,
                CreationDate = club.CreationDate,
                Address = club.Address,
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
        public async Task<IActionResult> Edit([Bind("Id,Name,Email,PhoneNumber,AddressString,ClubTranslationsAbout,ClubTranslationsTerms,CreationDate,File,RemoveImage,ModalitiesIds")] EditModel club)
        {
            //check model state
            ViewBag.CultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            ViewBag.Modalities = new MultiSelectList(await _clubService.GetModalitiesToSelectList(), "Id", "Name", club.ModalitiesIds);
            ViewBag.Languages = new List<CultureInfo> { new("en-US"), new("pt-PT") };
            if (!ModelState.IsValid) return View(club);

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check roles
            if (!_clubService.IsClubAdmin(role)) return View("CustomError", "Error_Unauthorized");

            //check if the club being edited is the same
            //as the one the user has selected
            if (role.ClubId != club.Id) return View("CustomError", "Error_Unauthorized");

            //get club infos
            var actualClub = await _clubService.GetClub(role.ClubId);
            if (actualClub == null) return View("CustomError", "Error_NotFound");

            await _clubService.UpdateClubModalities(actualClub, club.ModalitiesIds!);

            if (club.AddressString != null)
            {
                Address newLocation = JsonConvert.DeserializeObject<Address>(club.AddressString);

                if (actualClub.AddressId != null)
                {
                    // update Address
                    await _clubService.UpdateClubAddress(newLocation, (int)actualClub.AddressId);
                }
                else
                {
                    //create address
                    await _clubService.CreateAddress(newLocation, actualClub.Id);
                }

            }

            //updates to the settings of club
            actualClub.Name = club.Name;
            actualClub.Email = club.Email;
            actualClub.PhoneNumber = club.PhoneNumber;

            //update translations
            ICollection<ClubTranslations> clubTranslationsFromFrontend = new List<ClubTranslations>(club.ClubTranslationsAbout);
            clubTranslationsFromFrontend.AddRange(club.ClubTranslationsTerms);

            await _translationService.Translate(club.ClubTranslationsAbout);
            await _translationService.Translate(club.ClubTranslationsTerms);

            foreach (var translations in clubTranslationsFromFrontend)
            {
                var f = actualClub.ClubTranslations.FirstOrDefault(c => c.Id == translations.Id);
                if (f != null)
                {
                    f.Value = translations.Value;
                }
            }

            //update photo
            var result = await _clubService.UpdateClubPhoto(actualClub, club.RemoveImage, club.File);
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.ImageError = result;
                return View(club);
            }

            await _clubService.UpdateClub(actualClub);
            await _paymentService.UpdateProductClubMembership(actualClub.ClubPaymentSettings);

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

            public ICollection<ClubTranslations>? ClubTranslationsAbout { get; set; }
            public ICollection<ClubTranslations>? ClubTranslationsTerms { get; set; }

            public DateTime CreationDate { get; set; }

            public int? PhotographyId { get; set; }

            public BlobDto? Photography { get; set; }

            public string? AddressString { get; set; }

            public Address? Address { get; set; }

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
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

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
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

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
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

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

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            UsersRoleClub? userRoleToBeRomoved = await _clubService.GetUserRoleClubFromId((int)usersRoleClubId);

            //role with specified id does not exist or is club admin (which cant be removed)
            if (userRoleToBeRomoved == null || userRoleToBeRomoved.RoleId == 50) return View("CustomError", "Error_Unauthorized");

            //if the user role to be removed does not belong to the club of the user
            //that is making the request, then return unauthorized
            if (userRoleToBeRomoved.ClubId != role.ClubId) return View("CustomError", "Error_Unauthorized");

            //prevent users that arent club admin from removing club secretary (or higher)
            if (userRoleToBeRomoved.RoleId >= 40 && role.RoleId < 50) return View("CustomError", "Error_Unauthorized");

            //Check if the user is trainer and if so, transfer all teams to admin
            if(userRoleToBeRomoved.RoleId == 30 || userRoleToBeRomoved.RoleId == 40)
            {
                var adminRole = await _clubService.GetAdminRole(userRoleToBeRomoved.ClubId);
                await _teamService.TransferOwnerOfAllTeams(userRoleToBeRomoved.UserId, adminRole.UserId);
                //var teams = await _teamService.GetTeamsByTrainer(userRoleToBeRomoved.UserId);

                //if (teams != null && teams.Any())
                //{
                //    

                //    _teamService.TransferOwnerOfAllTeams(userRoleToBeRomoved.UserId, adminRole.UserId));
                //}
            }

            //Check if the user is athlete and if so, remove all the athlete data (All teams of this club)
            if (userRoleToBeRomoved.RoleId == 20)
            {
                var teams = await _teamService.GetTeamsByAthlete(userRoleToBeRomoved.UserId, userRoleToBeRomoved.ClubId);

                if (teams != null && teams.Any())
                {
                    var user = await _userService.GetUser(userRoleToBeRomoved.UserId);

                    var tasks = teams.Select(team => _teamService.RemoveAthlete(team, user));
                    await Task.WhenAll(tasks);
                }
            }
            
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
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            var roles = (await _clubService.GetRoles()).Select(s => new RoleClub { Id = s.Id, RoleName = _stringLocalizer[s.RoleName] });

            ViewBag.Roles = new SelectList(roles, "Id", "RoleName");

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
            if (!ModelState.IsValid || codeClub.ExpireDate.Date < DateTime.Now.Date) return RedirectToAction("Codes");
            
            var validRoles = await _clubService.GetRoles(); //excludes partners and club admin
            if (!validRoles.Any(r => r.Id == codeClub.RoleId)) return RedirectToAction("Codes");

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            CodeClub codeToBeCreated = new CodeClub
            {
                ClubId = role.ClubId,
                CreatedByUserId = _applicationContextService.UserId,
                RoleId = codeClub.RoleId,
                ExpireDate = codeClub.ExpireDate,
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
            public DateTime ExpireDate { get; set; }
        }

        /// <summary>
        /// Gets the list of codes for the club with status
        /// </summary>
        /// <param name="approveCode"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IActionResult> Codes(string? approveCode, int? code)
        {
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check user role
            if (!_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.isAdmin = _clubService.IsClubAdmin(role);

            var slots = await _clubService.ClubAthleteSlots(role.ClubId);

            ViewBag.Slots = $"{slots.UsedSlots}/{slots.TotalSlots}";

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
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

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
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check role
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            //Only Manager or Trainer can create teams
            ViewBag.CanCreate = (_clubService.IsClubManager(role) | _clubService.IsClubTrainer(role)) ? true : false;

            return View(await _teamService.GetTeams(role.ClubId));
        }

        /// <summary>
        /// Create a new team (view)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreateTeam()
        {
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check role Trainer
            //if its trainer will use userId
            if (_clubService.IsClubTrainer(role))
            {
                ViewBag.IsManager = false;
                ViewBag.TrainerId = _applicationContextService.UserId;
            }

            //if its admin will send trainers IDs to selectList
            else if (_clubService.IsClubManager(role))
            {
                ViewBag.IsManager = true;
                ViewBag.ClubTrainers = new SelectList(await _clubService.GetClubStaff(role.ClubId), "UserId", "User.FullName");
            }
            else
            {
                return View("CustomError", "Error_Unauthorized");
            }
            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(role.ClubId), "Id", "Name");

            return View();
        }

        /// <summary>
        /// Create a new team (post)
        /// </summary>
        /// <param name="team">Clube to be created</param>
        /// <returns>Index View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeam([Bind("Id,Name,ModalityId, TrainerId")] TeamModel team)
        {
            if (!ModelState.IsValid) return View(team);

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check role Trainer
            if (!_clubService.IsClubTrainer(role) && !_clubService.IsClubManager(role)) return View("CustomError", "Error_Unauthorized");

            await _teamService.CreateTeam(new Team { Name = team.Name, ModalityId = team.ModalityId, TrainerId = team.TrainerId, ClubId = role.ClubId });

            return RedirectToAction(nameof(TeamList));

        }

        public class TeamModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Error_Required")]
            [StringLength(40, ErrorMessage = "Error_Length", MinimumLength = 2)]
            [Display(Name = "Team Name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Error_Required")]
            [Display(Name = "Modality")]
            public int ModalityId { get; set; }

            [Required(ErrorMessage = "Error_Required")]
            [Display(Name = "Trainer")]
            public string TrainerId { get; set; }

            public User? Trainer { get; set; }

            public ICollection<User>? Athletes { get; set; }

            public static TeamModel ConvertTeam(Team team)
            {
                return new TeamModel
                {
                    Id = team.Id,
                    Name = team.Name,
                    ModalityId = team.ModalityId,
                    TrainerId = team.TrainerId,
                    Trainer = team.Trainer,
                    Athletes = team.Athletes
                };
            }
        }


        /// <summary>
        /// Edit team view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditTeam(int? id)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //get the team
            var team = await _teamService.GetTeam((int)id);

            if (team == null) return View("CustomError", "Error_NotFound");

            //check role
            if (!_clubService.IsClubStaff(role) || team.ClubId != role.ClubId || (team.TrainerId != role.UserId && !_clubService.IsClubManager(role))) return View("CustomError", "Error_Unauthorized");

            //check role Trainer
            //if its trainer will use userId
            if (_clubService.IsClubTrainer(role))
            {
                ViewBag.IsManager = false;
            }

            //if its admin will send trainers IDs to selectList
            else if (_clubService.IsClubManager(role))
            {
                ViewBag.IsManager = true;
                ViewBag.ClubTrainers = new SelectList(await _clubService.GetClubStaff(role.ClubId), "UserId", "User.FullName");
            }

            //get the club
            var club = await _clubService.GetClub(role.ClubId);

            if (club == null) return View("CustomError", "Error_NotFound");

            //viewbag that have the modalities of the club
            ViewBag.Modalities = new SelectList(await _clubService.GetClubModalities(club.Id), "Id", "Name");


            return View(TeamModel.ConvertTeam(team));
        }

        /// <summary>
        /// Saves edited team
        /// </summary>
        /// <param name="id"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeam(int? id, [Bind("Id,Name,TrainerId,ModalityId")] TeamModel team)
        {
            if (id == null) return View("CustomError", "Error_NotFound");

            //check model state
            if (!ModelState.IsValid) return View(team);

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //Update Team Atributes
            Team teamToUpdate = await _teamService.GetTeam((int)id);
            if (teamToUpdate == null) return View("CustomError", "Error_NotFound");

            //check roles
            if (!_clubService.IsClubStaff(role) || teamToUpdate.ClubId != role.ClubId) return View("CustomError", "Error_Unauthorized");


            //Check if team have modification
            if (teamToUpdate.Name == team.Name && teamToUpdate.ModalityId == team.ModalityId && teamToUpdate.TrainerId == team.TrainerId) return RedirectToAction(nameof(TeamList));

            teamToUpdate.Name = team.Name;
            teamToUpdate.ModalityId = team.ModalityId;
            teamToUpdate.TrainerId = team.TrainerId;

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
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check user role
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            Team? team = await _teamService.GetTeam(id);

            if (team == null) return View("CustomError", "Error_NotFound");

            //check user role
            if (!_clubService.IsClubStaff(role) || team.ClubId != role.ClubId) return View("CustomError", "Error_Unauthorized");

            //if he is trainer need to be the trainer of the team
            if (_clubService.IsClubTrainer(role) && team.TrainerId != _applicationContextService.UserId) return View("CustomError", "Error_Unauthorized");

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
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            Team? team = await _teamService.GetTeam(id);

            if (team == null) return View("CustomError", "Error_NotFound");

            //check user role
            if (!_clubService.IsClubStaff(role) || team.ClubId != role.ClubId) return View("CustomError", "Error_Unauthorized");

            //if he is trainer need to be the trainer of the team
            if (_clubService.IsClubTrainer(role) && team.TrainerId != _applicationContextService.UserId) return View("CustomError", "Error_Unauthorized");

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

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //check if athlete is on team
            Team team = await _teamService.GetTeam((int)teamId);

            if (team == null) return View("CustomError", "Error_NotFound");

            //check user role
            if (!_clubService.IsClubStaff(role) || team.ClubId != role.ClubId) return View("CustomError", "Error_Unauthorized");

            User athleteToRemove = await _userService.GetUser(athleteId);
            //Chekc if athlete exists
            if (athleteToRemove == null) return View("CustomError", "Error_NotFound");


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

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

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

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //Check if is staff
            if (!_clubService.IsClubStaff(role) || team.ClubId != role.ClubId) return View("CustomError", "Error_Unauthorized");

            //Check if is trainer
            if (_clubService.IsClubTrainer(role) && team.TrainerId != _applicationContextService.UserId) return View("CustomError", "Error_Unauthorized");

            await _teamService.DeleteTeam(team);

            return RedirectToAction(nameof(TeamList));
        }

        /// <summary>
        /// Gets the teams that an athlete is in
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> MyTeams()
        {
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //Check if is athlete
            if (!_clubService.IsClubAthlete(role)) return View("CustomError", "Error_Unauthorized");

            var teams = await _teamService.GetTeamsByAthlete(_applicationContextService.UserId);

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

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //Check if is staff
            if (!_clubService.IsClubMember(_applicationContextService.UserId, role.ClubId)) return PartialView("_CustomErrorPartial", "Error_Unauthorized");

            //check if the athlete belongs to the club
            if (!_clubService.IsClubMember(id, role.ClubId)) return PartialView("_CustomErrorPartial", "Error_Unauthorized");

            var user = await _userService.GetUser(id);
            if (user == null) return PartialView("_CustomErrorPartial", "Error_NotFound");

            var userRole = await _clubService.GetUserRoleInClub(user.Id, role.ClubId);
            if (userRole.RoleId > role.RoleId) return PartialView("_CustomErrorPartial", "Error_Unauthorized");

            var bio = await _userService.GetLastBioimpedance(user.Id);

            if (bio != null)
            {
                ViewBag.HaveBio = true;
                ViewBag.Weight = bio.Weight == null ? "" : bio.Weight;
                ViewBag.Height = bio.Height == null ? "" : bio.Height;
                ViewBag.Hydration = bio.Hydration == null ? "" : bio.Hydration.ToString();
                ViewBag.FatMass = bio.FatMass == null ? "" : bio.FatMass.ToString();
                ViewBag.LeanMass = bio.LeanMass == null ? "" : bio.LeanMass.ToString();
                ViewBag.MuscleMass = bio.MuscleMass == null ? "" : bio.MuscleMass.ToString();
                ViewBag.BasalMetabolism = bio.BasalMetabolism == null ? "" : bio.BasalMetabolism.ToString();
                ViewBag.ViceralFat = bio.ViceralFat == null ? "" : bio.ViceralFat.ToString();
                ViewBag.LastUpdateDate = bio.LastUpdateDate;
            }
            else
                ViewBag.HaveBio = false;



            return PartialView("_PartialUserDetails", user);
        }

        public async Task<IActionResult> PaymentSettings()
        {
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //Check if is staff
            if (!_clubService.IsClubAdmin(role)) return View("CustomError", "Error_Unauthorized");

            ViewBag.Frequency = new SelectList(from SubscriptionFrequency sf in Enum.GetValues(typeof(SubscriptionFrequency)) select new { Id = (int)sf, Name = _stringLocalizer[sf.ToString()] }, "Id", "Name");

            var settings = await _clubService.GetClubPaymentSettings(role.ClubId);

            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentSettings(ClubPaymentSettings paymentSettings)
        {
            ViewBag.Frequency = new SelectList(from SubscriptionFrequency sf in Enum.GetValues(typeof(SubscriptionFrequency)) select new { Id = (int)sf, Name = _stringLocalizer[sf.ToString()] }, "Id", "Name");

            if (!ModelState.IsValid) return View(paymentSettings);

            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //Check if is staff
            if (!_clubService.IsClubAdmin(role) || paymentSettings.ClubPaymentSettingsId != role.ClubId) return View("CustomError", "Error_Unauthorized");

            try
            {
                await _paymentService.TestAccount(paymentSettings.AccountId ?? "", paymentSettings.AccountKey ?? "");
                paymentSettings.ValidCredentials = true;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(paymentSettings);
            }

            var updated = await _clubService.UpdateClubPaymentSettings(paymentSettings);
            await _paymentService.UpdateProductClubMembership(updated);

            return RedirectToAction(nameof(PaymentSettings));
        }

        public async Task<IActionResult> PaymentsRecieved()
        {
            //get the user selected role
            UsersRoleClub role = _applicationContextService.UserRole;

            //Check if is staff
            if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

            var payments = await _paymentService.GetClubPayments(role.ClubId);

            return View(payments);
        }
    }
}
