using FakeItEasy;
using FakeItEasy.Creation;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Controllers;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PaymentService;
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
        private readonly IUrlHelper _urlHelper;

        public MyClubControllerTests()
        {
            _userManager = A.Fake<UserManager<User>>();
            _clubService = A.Fake<IClubService>();
            _userService = A.Fake<IUserService>();
            _teamService = A.Fake<ITeamService>();
            _translationService = A.Fake<ITranslationService>();
            _paymentService = A.Fake<IPaymentService>();
            _urlHelper = A.Fake<IUrlHelper>();

            //SUT (system under test)
            _controller = new MyClubController(_userManager, _clubService, _userService, _teamService, _translationService, _paymentService, _urlHelper);
        }
        
        [Fact]
        public async Task MyClubController_Index_ReturnsSuccess()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_Index_ClubIdEqualsZero()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 0 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_Edit_ReturnsIsNotClubAdmin()
        {
            // Arrange
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { ClubId = 1 };
            var clube = A.Fake<EditModel>();
            var club = A.Fake<Club>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);

            // Act
            var result = await _controller.Edit(clube);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task MyClubController_Edit_Post_ReturnsIsNotClubAdmin()
        {
            // Arrange
            var role = A.Fake<UsersRoleClub>();
            var clube = A.Fake<EditModel>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { ClubId = 1 };
            var clube = new EditModel { Id = 2 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { ClubId = 1 };
            var clube = new EditModel { Id = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var userRoleToBeRomoved = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.RemoveUser(1, "AthletesList");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        
        [Fact]
        public async Task MyClubController_RemoveUser_ReturnsUserRoleToBeRomovedNull()
        {
            // Arrange
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var userRoleToBeRomoved = new UsersRoleClub { RoleId = 50 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { ClubId = 4 };
            var userRoleToBeRomoved = new UsersRoleClub { ClubId = 3 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { RoleId = 40 };
            var userRoleToBeRomoved = new UsersRoleClub { RoleId = 40 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var code = A.Fake<CodeClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GenerateCode(A<CodeClub>._)).Returns(code);

            // Act
            var result = await _controller.CreateCode(A.Fake<CreateCodeModel>());

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Codes");
        }

        [Fact]
        public async Task MyClubController_CreateCode_Post_ReturnsIsNotClubManager()
        {
            // Arrange
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _controller.Codes("code",1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_Codes_ReturnsIsNotClubManager()
        {
            // Arrange
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            var team = A.Fake<Team>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            var team = new Team{ ModalityId = 1};
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var clube = A.Fake<Club>();
            var team = new Team { ModalityId = 1 , Name = "teste" };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            var users = new List<User>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var team = new Team { TrainerId = "1"};
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            var users = new List<User>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var team = new Team { TrainerId = "1"};
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var user = A.Fake<User>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() { user } };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var user = A.Fake<User>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var user = A.Fake<User>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "1", Athletes = new List<User>() };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            var team = new Team { ModalityId = 1, TrainerId = "", Athletes = new List<User>() };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
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
            var role = A.Fake<UsersRoleClub>();
            var teams = new List<Team>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { RoleId = 1};
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { RoleId = 1 };
            var user = A.Fake<User>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { RoleId = 2 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubMember(A<string>._, A<int>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubMember(id, A<int>._)).Returns(true);
            A.CallTo(() => _clubService.GetUserRoleInClub(A<string>._, A<int>._)).Returns(1);

            // Act
            var result = await _controller.UserDetails(id);

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_PartialUserDetails");
        }

        [Fact]
        public void MyClubController_NewAddress_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _controller.NewAddress();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_ReceiveAddress_Post_ReturnsSuccess()
        {
            // Arrange
            var role = new UsersRoleClub { RoleId = 2 };
            var club = A.Fake<Club>();
            var address = new Models.Address
            { 
                Id = 1,
                Street = "Street",
                City = "City",
                ZipCode = "ZipCode",
                Country = "Country",
                CoordinateX = 1,
                CoordinateY = 1
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);


            // Act
            var result = await _controller.ReceiveAddress(address);

            // Assert
            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task MyClubController_ReceiveAddress_Post_ReturnsUpdate()
        {
            // Arrange
            var role = new UsersRoleClub { RoleId = 2 };
            var club = A.Fake<Club>();
            club.AddressId = 1;
            var address = new Models.Address
            {
                Id = 1,
                Street = "Street",
                City = "City",
                ZipCode = "ZipCode",
                Country = "Country",
                CoordinateX = 1,
                CoordinateY = 1
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);


            // Act
            var result = await _controller.ReceiveAddress(address);

            // Assert
            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task MyClubController_ReceiveAddress_Post_ReturnsIsNotClubAdmin()
        {
            // Arrange
            var role = new UsersRoleClub { RoleId = 2 };
            var club = A.Fake<Club>();
            var address = new Models.Address
            {
                Id = 1,
                Street = "Street",
                City = "City",
                ZipCode = "ZipCode",
                Country = "Country",
                CoordinateX = 1,
                CoordinateY = 1
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(false);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);


            // Act
            var result = await _controller.ReceiveAddress(address);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_ReceiveAddress_Post_ReturnsClubNull()
        {
            // Arrange
            var role = new UsersRoleClub { RoleId = 2 };
            var address = new Models.Address
            {
                Id = 1,
                Street = "Street",
                City = "City",
                ZipCode = "ZipCode",
                Country = "Country",
                CoordinateX = 1,
                CoordinateY = 1
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(Task.FromResult<Club>(null));


            // Act
            var result = await _controller.ReceiveAddress(address);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task MyClubController_PaymentSettings_ReturnsSuccess()
        {
            // Arrange
            var role = new UsersRoleClub { RoleId = 2 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { RoleId = 2 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { RoleId = 2, ClubId = 2 };
            var paymentSettings = new ClubPaymentSettings()
            {
                ClubPaymentSettingsId = 2,
                AccountId = "AccountId",
                AccountKey = "AccountKey"
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
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
            var role = new UsersRoleClub { RoleId = 2, ClubId = 2 };
            var paymentSettings = new ClubPaymentSettings()
            {
                ClubPaymentSettingsId = 2,
                AccountId = "AccountId",
                AccountKey = "AccountKey"
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubAdmin(A<UsersRoleClub>._)).Returns(false);
            A.CallTo(() => _clubService.UpdateClubPaymentSettings(A<ClubPaymentSettings>._)).Returns(A.Fake<ClubPaymentSettings>());

            // Act
            var result = await _controller.PaymentSettings(paymentSettings);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_MissingPayment_ReturnsSuccess()
        {
            // Arrange
            var role = new UsersRoleClub { RoleId = 2, ClubId = 2 };
            var paymentSettings = new ClubPaymentSettings()
            {
                ClubPaymentSettingsId = 2,
                AccountId = "AccountId",
                AccountKey = "AccountKey"
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);

            // Act
            var result = await _controller.MissingPayment();

            // Assert
            result.Should().BeOfType<ViewResult>();
            
        }

        [Fact]
        public async Task MyClubController_MissingPayment_ReturnsClubIdZero()
        {
            // Arrange
            var role = new UsersRoleClub { RoleId = 2, ClubId = 0 };
            var paymentSettings = new ClubPaymentSettings()
            {
                ClubPaymentSettingsId = 2,
                AccountId = "AccountId",
                AccountKey = "AccountKey"
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);

            // Act
            var result = await _controller.MissingPayment();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }



    }
}
