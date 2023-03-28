using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using SCManagement.Controllers;
using SCManagement.Models;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.ClubService;
using SCManagement.Services.PaymentService;
using SCManagement.Services.PlansService;
using SCManagement.Services.TeamService;
using SCManagement.Services.TranslationService;
using SCManagement.Services.UserService;
using SCManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using static SCManagement.Controllers.MyZoneController;

namespace SCManagement.Tests.Controller
{
    public class MyZoneControllerTests
    {
        private readonly MyZoneController _controller;
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly ITranslationService _translationService;
        private readonly IPaymentService _paymentService;
        private readonly ApplicationContextService _applicationContextService;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly IAzureStorage _azureStorage;
        private readonly IPlanService _planService;

        public MyZoneControllerTests()
        {
            _userManager = A.Fake<UserManager<User>>();
            _clubService = A.Fake<IClubService>();
            _userService = A.Fake<IUserService>();
            _teamService = A.Fake<ITeamService>();
            _translationService = A.Fake<ITranslationService>();
            _paymentService = A.Fake<IPaymentService>();
            _applicationContextService = A.Fake<ApplicationContextService>();
            _stringLocalizer = A.Fake<IStringLocalizer<SharedResource>>();
            _azureStorage = A.Fake<IAzureStorage>();
            _planService = A.Fake<IPlanService>();

            //SUT (system under test)
            _controller = new MyZoneController(_userManager, _clubService, _userService, _teamService, _translationService, _paymentService, _applicationContextService, _stringLocalizer, _azureStorage, _planService);
        }

        [Fact]
        public async Task MyClubController_MyZone_ReturnsIsNotClubAthlete()
        {
            // Arrange
            _applicationContextService.UserRole = new UsersRoleClub { RoleId = 30 };
            A.CallTo(() => _clubService.IsClubAthlete(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_CreateBioimpedance_Post_ReturnsSuccess()
        {
            //Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubAthlete(A<UsersRoleClub>._)).Returns(true);

            var bioimpedance = new Bioimpedance
            {
                Weight = "80kg",
                Height = "185cm"
            };

            // Act
            var result = await _controller.CreateBioimpedance(A.Fake<BioimpedanceModel>());

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");

        }

        [Fact]
        public async Task MyClubController_CreateBioimpedance_Post_ReturnsIsNotClubAthlete()
        {
            //Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubAthlete(A<UsersRoleClub>._)).Returns(false);

            var bioimpedance = new Bioimpedance
            {
                Weight = "80kg",
                Height = "185cm"
            };

            // Act
            var result = await _controller.CreateBioimpedance(A.Fake<BioimpedanceModel>());

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_UpdateBioimpedance_ReturnsSuccess()
        {
            {
                // Arrange
                _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
                A.CallTo(() => _clubService.IsClubAthlete(A<UsersRoleClub>._)).Returns(true);
                A.CallTo(() => _userService.GetBioimpedances(A<string>._)).Returns(A.Fake<IEnumerable<Bioimpedance>>());

                // Act
                var result = await _controller.UpdateBioimpedance();

                // Assert
                result.Should().BeOfType<ViewResult>();
            }
        }

        [Fact]
        public async Task MyClubController_UpdateBioimpedance_ReturnsIsNotClubAthlete()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubAthlete(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.UpdateBioimpedance();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task MyClubController_UpdateBioimpedance_ReturnsDontHaveBioimpedance()
        {
            // Arrange
            _applicationContextService.UserRole = A.Fake<UsersRoleClub>();
            A.CallTo(() => _clubService.IsClubAthlete(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _userService.GetLastBioimpedance(A<string>._)).Returns(Task.FromResult<Bioimpedance>(null));

            // Act
            var result = await _controller.UpdateBioimpedance();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_DontHaveBioimpedance");
        }

    }
}
