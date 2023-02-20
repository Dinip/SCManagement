using FakeItEasy;
using FakeItEasy.Creation;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCManagement.Controllers;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.Location;
using SCManagement.Services.TeamService;
using SCManagement.Services.TranslationService;
using SCManagement.Services.UserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SCManagement.Controllers.MyClubController;

namespace SCManagement.Tests.Controller
{
    public class UsersRoleClubFakeOptionsBuilder : FakeOptionsBuilder<UsersRoleClub>
    {
        protected override void BuildOptions(IFakeOptions<UsersRoleClub> options)
        {
            options.ConfigureFake(fake =>
            {
                fake.ClubId = 1;
            });
        }
    }

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
        private readonly ILocationService _locationService;
        private readonly ITranslationService _translationService;

        public MyClubControllerTests()
        {
            _userManager = A.Fake<UserManager<User>>();
            _clubService = A.Fake<IClubService>();
            _userService = A.Fake<IUserService>();
            _teamService = A.Fake<ITeamService>();
            _locationService = A.Fake<ILocationService>();
            _translationService = A.Fake<ITranslationService>();

            //SUT (system under test)
            _controller = new MyClubController(_userManager, _clubService, _userService, _teamService, _locationService, _translationService);
        }
        
        [Fact]
        public async Task MyClubController_Index_ReturnsSuccess()
        {
            // Arrange
            var userId = "1";
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(userId)).Returns(role);

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MyClubController_Edit_ReturnsErrorRole()
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
            var role = A.Fake<UsersRoleClub>();
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
        public async Task MyClubController_CreateCode_ReturnsSuccess()
        {
            // Arrange
            var role = A.Fake<UsersRoleClub>();
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubManager(A<UsersRoleClub>._)).Returns(true);
            // Act
            var result = await _controller.CreateCode();

            // Assert
            result.Should().BeOfType<PartialViewResult>();
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


    }
}
