using FakeItEasy;
using FakeItEasy.Creation;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SCManagement.Controllers;
using SCManagement.Models;
using SCManagement.Services;
using SCManagement.Services.ClubService;
using SCManagement.Services.ClubService.Models;
using SCManagement.Services.PaymentService;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.TeamService;
using SCManagement.Services.TranslationService;
using SCManagement.Services.UserService;
using static SCManagement.Controllers.MyClubController;

namespace SCManagement.Tests.Controller
{

    public class ClubFakeOptionsBuilder : FakeOptionsBuilder<Club>
    {
        protected override void BuildOptions(IFakeOptions<Club> options)
        {
            options.ConfigureFake(fake =>
            {
                fake.Id = 1;
                fake.ClubTranslations = new List<ClubTranslations>();
                fake.Modalities = new List<Modality>();
            });
        }
    }

    public class EditModelFakeOptionsBuilder : FakeOptionsBuilder<EditModel>
    {
        protected override void BuildOptions(IFakeOptions<EditModel> options)
        {
            options.ConfigureFake(fake =>
            {
                fake.Id = 1;
                fake.ClubTranslationsAbout = new List<ClubTranslations>();
                fake.ClubTranslationsTerms = new List<ClubTranslations>();
                fake.ModalitiesIds = new List<int>();
                
            });
        }
    }



    public class MyClubControllerTests
    {
        private readonly MyClubController _controller;
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly ITranslationService _translationService;
        private readonly IPaymentService _paymentService;
        private readonly ApplicationContextService _applicationContextService;

        public MyClubControllerTests()
        {
            _userManager = A.Fake<UserManager<User>>();
            _clubService = A.Fake<IClubService>();
            _userService = A.Fake<IUserService>();
            _teamService = A.Fake<ITeamService>();
            _translationService = A.Fake<ITranslationService>();
            _paymentService = A.Fake<IPaymentService>();
            _applicationContextService = A.Fake<ApplicationContextService>();

            //SUT (system under test)
            _controller = new MyClubController(_userManager, _clubService, _userService, _teamService, _translationService, _paymentService, _applicationContextService);
        }

        [Fact]
        public async Task MyClubController_Unavailable_ReturnsSuccess()
        { 
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            var club = new Club() { Id = 1, Status = ClubStatus.Active };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);
            A.CallTo(() => _clubService.GetClubStatus(A<int>._)).Returns(ClubStatus.Active);

            // Act
            var result = await _controller.Unavailable();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task MyClubController_Unavailable_ReturnsClubIdEQZero()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 0 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);

            // Act
            var result = await _controller.Unavailable();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task MyClubController_Unavailable_ReturnsUnavailable()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            var club = new Club() { Id = 1, Status = ClubStatus.Waiting_Payment };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);

            // Act
            var result = await _controller.Unavailable();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Unavailable");
        }

        [Fact]
        public async Task MyClubController_Index_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 1 };

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_Index_ReturnsRoleIdEqualsZero()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 0 };

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_Edit_ReturnsIsNotClubAdmin()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(false);
            
            // Act
            var result = await _controller.Edit();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_Edit_ReturnsClubNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(Task.FromResult<Club>(null));
            
            // Act
            var result = await _controller.Edit();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_Edit_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(clube);

            // Act
            var result = await _controller.Edit();

            // Assert
            result.Should().BeOfType<ViewResult>();

        }

        [Fact]
        public async Task MyClubController_Edit_Post_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { ClubId = 1 };
            var clubFrontEnd = A.Fake<EditModel>();
            clubFrontEnd.ClubTranslationsTerms = new List<ClubTranslations>()
            {
                new ClubTranslations
                {
                    Id = 1,
                    ClubId = 1,
                    Value = "Olá",
                    Language = "pt-PT",
                    Atribute = "TermsAndConditions"
                }
            };
            var club = A.Fake<Club>();
            club.ClubTranslations = new List<ClubTranslations>() 
            {
                new ClubTranslations
                {
                    Id = 1,
                    ClubId = 1,
                    Value = "",
                    Language = "pt-PT",
                    Atribute = "TermsAndConditions"
                }
            };
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);

            // Act
            var result = await _controller.Edit(clubFrontEnd);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task MyClubController_Edit_Post_ReturnsIsNotClubAdmin()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var clube = A.Fake<EditModel>();
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.Edit(clube);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_Edit_Post_ReturnsRoleDiffClub()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { ClubId = 1 };
            var clube = new EditModel { Id = 2 };
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.Edit(clube);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_Edit_Post_ReturnsActualClubEqualsNull()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { ClubId = 1 };
            var clube = new EditModel { Id = 1};
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(Task.FromResult<Club>(null));

            // Act
            var result = await _controller.Edit(clube);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_PartnersList_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.PartnersList();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_PartnersList_ReturnsIsNotClubManager()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.PartnersList();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_StaffList_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.StaffList();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_StaffList_ReturnsIsNotClubManager()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.StaffList();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_AthletesList_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.AthletesList();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_AthletesList_ReturnsIsNotClubManager()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.AthletesList();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_RemoveUser_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var userRoleToBeRomoved = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetUserRoleClubFromId(A<int>._)).Returns(userRoleToBeRomoved);

            // Act
            var result = await _controller.RemoveUser(1, "AthletesList");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public async Task MyClubController_RemoveUser_ReturnsUsersRoleClubIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.RemoveUser(null, "AthletesList");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_RemoveUser_ReturnsIsNotClubManager()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.RemoveUser(1, "AthletesList");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        
        [Fact]
        public async Task MyClubController_RemoveUser_ReturnsUserRoleToBeRemovedNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetUserRoleClubFromId(A<int>._)).Returns(Task.FromResult<UsersRoleClub>(null));


            // Act
            var result = await _controller.RemoveUser(1, "AthletesList");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_RemoveUser_ReturnsUserRoleToBeRomovedIsClubAdmin()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var userRoleToBeRomoved = new UsersRoleClub { RoleId = 50 };
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetUserRoleClubFromId(A<int>._)).Returns(userRoleToBeRomoved);


            // Act
            var result = await _controller.RemoveUser(1, "AthletesList");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_RemoveUser_ReturnsUserRoleToBeRomovedDiffClubId()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { ClubId = 4 };
            var userRoleToBeRomoved = new UsersRoleClub { ClubId = 3 };
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetUserRoleClubFromId(A<int>._)).Returns(userRoleToBeRomoved);


            // Act
            var result = await _controller.RemoveUser(1, "AthletesList");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_RemoveUser_ReturnsPreventRemove()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 40 };
            var userRoleToBeRomoved = new UsersRoleClub { RoleId = 40 };
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetUserRoleClubFromId(A<int>._)).Returns(userRoleToBeRomoved);


            // Act
            var result = await _controller.RemoveUser(1, "AthletesList");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_CreateCode_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            // Act
            var result = await _controller.CreateCode();

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_PartialCreateCode");
        }

        [Fact]
        public async Task MyClubController_CreateCode_ReturnsIsNotClubManager()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(false);
            // Act
            var result = await _controller.CreateCode();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_CreateCode_Post_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { ClubId = 1 };
            var code = new CreateCodeModel { ExpireDate = DateTime.Now.AddDays(1)};
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GenerateCode(A<CodeClub>._)).Returns(A.Fake<CodeClub>());

            // Act
            var result = await _controller.CreateCode(code);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Codes");
        }

        [Fact]
        public async Task MyClubController_CreateCode_Post_ReturnsIsNotClubManager()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(false);
            
            // Act
            var result = await _controller.CreateCode(A.Fake<CreateCodeModel>());

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_Codes_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.ClubAthleteSlots(A<int>._)).Returns(A.Fake<ClubSlots>());

            // Act
            var result = await _controller.Codes("code",1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_Codes_ReturnsIsNotClubManager()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.ClubAthleteSlots(A<int>._)).Returns(A.Fake<ClubSlots>());
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.Codes("code", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_SendCodeEmail_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.SendCodeEmail(1, "a@gmail.com");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Codes");
        }

        [Fact]
        public async Task MyClubController_SendCodeEmail_ReturnsIsNotClubManager()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.SendCodeEmail(1, "a@gmail.com");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_TeamList_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.TeamList();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_TeamList_ReturnsIsNotClubManager()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.TeamList();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_CreateTeam_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.CreateTeam();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_CreateTeam_ReturnsError()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.CreateTeam();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_CreateTeam_Post_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.CreateTeam(A.Fake<TeamModel>());

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TeamList");
        }

        [Fact]
        public async Task MyClubController_CreateTeam_Post_ReturnsIsNotClubTrainerAndIsNotClubAdmin()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.CreateTeam(A.Fake<TeamModel>());

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_EditTeam_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            var team = A.Fake<Team>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(clube);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);

            // Act
            var result = await _controller.EditTeam(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_EditTeam_ReturnsIsNotClubStaff()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.EditTeam(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_EditTeam_ReturnsClubNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(Task.FromResult<Club>(null));

            // Act
            var result = await _controller.EditTeam(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_EditTeam_ReturnsTeamNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(clube);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(Task.FromResult<Team>(null));

            // Act
            var result = await _controller.EditTeam(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_EditTeam_Post_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            var team = new Team{ ModalityId = 1};
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(clube);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);

            // Act
            var result = await _controller.EditTeam(1, new TeamModel { ModalityId = 2 });

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TeamList");
        }

        [Fact]
        public async Task MyClubController_EditTeam_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.EditTeam(1, new TeamModel { ModalityId = 2 });

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_EditTeam_Post_ReturnsTeamNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(Task.FromResult<Team>(null));

            // Act
            var result = await _controller.EditTeam(1, new TeamModel { ModalityId = 2 });

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_EditTeam_Post_ReturnsNoUpdate()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            var team = new Team { ModalityId = 1 , Name = "teste" };
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(clube);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);

            // Act
            var result = await _controller.EditTeam(1, new TeamModel { ModalityId = 1, Name = "teste" });

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TeamList");
        }

        [Fact]
        public async Task MyClubController_AddTeamAthletes_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            var users = new List<User>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);
            A.CallTo(() => _clubService.GetAthletes(A<int>._)).Returns(users);

            // Act
            var result = await _controller.AddTeamAthletes(1);

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_PartialAddTeamAthletes");
        }

        [Fact]
        public async Task MyClubController_AddTeamAthletes_ReturnsIsNotClubStaff()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.AddTeamAthletes(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_AddTeamAthletes_ReturnsTeamNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(Task.FromResult<Team>(null));

            // Act
            var result = await _controller.AddTeamAthletes(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_AddTeamAthletes_ReturnsIsNotTeamTrainer()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var team = new Team { TrainerId = "1"};
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.AddTeamAthletes(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_AddTeamAthletes_Post_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);

            // Act
            var result = await _controller.AddTeamAthletes(1, new List<string>());

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("EditTeam");
        }

        [Fact]
        public async Task MyClubController_AddTeamAthletes_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.AddTeamAthletes(1, new List<string>());

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_AddTeamAthletes_Post_ReturnsTeamNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(Task.FromResult<Team>(null));

            // Act
            var result = await _controller.AddTeamAthletes(1, new List<string>());

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_AddTeamAthletes_Post_ReturnsIsNotTeamTrainer()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var team = new Team { TrainerId = "1"};
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.AddTeamAthletes(1, new List<string>());

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_RemoveAtheleFromTeam_Post_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var user = A.Fake<User>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() { user } };
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _userService.GetUser(A<string>._)).Returns(user);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);

            // Act
            var result = await _controller.RemoveAtheleFromTeam("andre",1, "Team");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("EditTeam");
        }

        [Fact]
        public async Task MyClubController_RemoveAtheleFromTeam_Post_ReturnsAthleteIdNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();

            // Act
            var result = await _controller.RemoveAtheleFromTeam(null, 1, "Team");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_RemoveAtheleFromTeam_Post_ReturnsTeamIdNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();

            // Act
            var result = await _controller.RemoveAtheleFromTeam("andre", null, "page1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_RemoveAtheleFromTeam_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.RemoveAtheleFromTeam("andre", 1, "Team");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_RemoveAtheleFromTeam_Post_ReturnsAthleteToRemoveNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _userService.GetUser(A<string>._)).Returns(Task.FromResult<User>(null));

            // Act
            var result = await _controller.RemoveAtheleFromTeam("andre", 1, "Team");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_RemoveAtheleFromTeam_Post_ReturnsTeamNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var user = A.Fake<User>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _userService.GetUser(A<string>._)).Returns(user);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(Task.FromResult<Team>(null));

            // Act
            var result = await _controller.RemoveAtheleFromTeam("andre", 1, "Team");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_RemoveAtheleFromTeam_Post_ReturnsAthleteToRemoveNotInAthletes()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var user = A.Fake<User>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _userService.GetUser(A<string>._)).Returns(user);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);

            // Act
            var result = await _controller.RemoveAtheleFromTeam("andre", 1, "Team");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_TeamDetails_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);
            A.CallTo(() => _clubService.IsClubMember(A<string>._, A<int>._)).Returns(true);

            // Act
            var result = await _controller.TeamDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_TeamDetails_ReturnsTeamNull()
        {
            // Arrange
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(Task.FromResult<Team>(null));


            // Act
            var result = await _controller.TeamDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_TeamDetails_ReturnsIsNotClubMember()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);
            A.CallTo(() => _clubService.IsClubMember(A<string>._,A<int>._)).Returns(false);

            // Act
            var result = await _controller.TeamDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_DeleteTeam_Post_ReturnsIsNotTeamTrainer()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "1", Athletes = new List<User>() };
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);

            // Act
            var result = await _controller.DeleteTeam(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_DeleteTeam_Post_ReturnsTeamNull()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(Task.FromResult<Team>(null));


            // Act
            var result = await _controller.DeleteTeam(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_DeleteTeam_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.DeleteTeam(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_DeleteTeam_Post_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(team);


            // Act
            var result = await _controller.DeleteTeam(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TeamList");
        }

        [Fact]
        public async Task MyClubController_MyTeams_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            var teams = new List<Team>();
            A.CallTo(() => _clubService.IsClubAthlete(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeamsByAthlete(A<string>._, A<int>._)).Returns(teams);


            // Act
            var result = await _controller.MyTeams();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_MyTeams_ReturnsIsNotClubAthlete()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubAthlete(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.MyTeams();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }


        [Fact]
        public async Task MyClubController_UserDetails_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.UserDetails(null);

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_CustomErrorPartial");
            result.Should().BeOfType<PartialViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_UserDetails_ReturnsUserIsNotClubMember()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubMember(A<string>._, A<int>._)).Returns(false);

            // Act
            var result = await _controller.UserDetails("1");

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_CustomErrorPartial");
            result.Should().BeOfType<PartialViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_UserDetails_ReturnsAthleteIsNotClubMember()
        {
            // Arrange
            var id = "1";
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubMember(A<string>._, A<int>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubMember(id, A<int>._)).Returns(false);

            // Act
            var result = await _controller.UserDetails(id);

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_CustomErrorPartial");
            result.Should().BeOfType<PartialViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_UserDetails_ReturnsUserNull()
        {
            // Arrange
            var id = "1";
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 1};
            A.CallTo(() => _clubService.IsClubMember(A<string>._, A<int>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubMember(id, A<int>._)).Returns(true);
            A.CallTo(() => _userService.GetUser(A<string>._)).Returns(Task.FromResult<User>(null));

            // Act
            var result = await _controller.UserDetails(id);

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_CustomErrorPartial");
            result.Should().BeOfType<PartialViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_UserDetails_ReturnsUserRoleIdHigher()
        {
            // Arrange
            var id = "1";
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 1 };
            var user = A.Fake<User>();
            A.CallTo(() => _clubService.IsClubMember(A<string>._, A<int>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubMember(id, A<int>._)).Returns(true);
            A.CallTo(() => _userService.GetUser(A<string>._)).Returns(user);
            A.CallTo(() => _clubService.GetUserRoleInClub(A<string>._, A<int>._)).Returns(2);

            // Act
            var result = await _controller.UserDetails(id);

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_CustomErrorPartial");
            result.Should().BeOfType<PartialViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_UserDetails_ReturnsSuccess()
        {
            // Arrange
            var id = "1";
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 2 };
            A.CallTo(() => _clubService.IsClubMember(A<string>._, A<int>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubMember(id, A<int>._)).Returns(true);
            A.CallTo(() => _clubService.GetUserRoleInClub(A<string>._, A<int>._)).Returns(1);

            // Act
            var result = await _controller.UserDetails(id);

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_PartialUserDetails");
        }
        

        [Fact]
        public async Task MyClubController_PaymentSettings_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 2 };
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClubPaymentSettings(A<int>._)).Returns(A.Fake<ClubPaymentSettings>());

            // Act
            var result = await _controller.PaymentSettings();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_PaymentSettings_ReturnsIsNotClubAdmin()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 2 };
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.PaymentSettings();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_PaymentSettings_Post_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 2, ClubId = 2 };
            var paymentSettings = new ClubPaymentSettings()
            {
                ClubPaymentSettingsId = 2,
                AccountId = "AccountId",
                AccountKey = "AccountKey"
            };
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.UpdateClubPaymentSettings(A<ClubPaymentSettings>._)).Returns(A.Fake<ClubPaymentSettings>());

            // Act
            var result = await _controller.PaymentSettings(paymentSettings);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_PaymentSettings_Post_ReturnsIsNotClubAdmin()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 2, ClubId = 2 };
            var paymentSettings = new ClubPaymentSettings()
            {
                ClubPaymentSettingsId = 2,
                AccountId = "AccountId",
                AccountKey = "AccountKey"
            };
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(false);
            A.CallTo(() => _clubService.UpdateClubPaymentSettings(A<ClubPaymentSettings>._)).Returns(A.Fake<ClubPaymentSettings>());

            // Act
            var result = await _controller.PaymentSettings(paymentSettings);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_PaymentsRecieved_ReturnsIsNotClubStaff()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.PaymentsRecieved();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_PaymentsRecieved_ReturnsSuccess()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _paymentService.GetClubPayments(A<int>._)).Returns(A.Fake<IEnumerable<Payment>>());

            // Act
            var result = await _controller.PaymentsRecieved();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }



    }
}
