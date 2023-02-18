using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using SCManagement.Controllers;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.UserService;

namespace SCManagement.Tests.Controller
{
    public class ClubsControllerTests
    {
        private readonly ClubsController _controller;
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;

        public ClubsControllerTests()
        {
            _userManager = A.Fake<UserManager<User>>();
            _clubService = A.Fake<IClubService>();
            _userService = A.Fake<IUserService>();

            //SUT (system under test)
            _controller = new ClubsController(_userManager, _clubService, _userService);
        }

        [Fact]
        public async Task ClubsController_Index_ReturnsSuccess()
        {
            // Arrange
            var clubs = A.Fake<IEnumerable<Club>>();
            A.CallTo(() => _clubService.GetClubs()).Returns(clubs);

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void ClubsController_Detail_ReturnsSuccess()
        {
            // Arrange
            var id = 1;
            var club = A.Fake<Club>();
            A.CallTo(() => _clubService.GetClub(id)).Returns(club);

            // Act
            var result = _controller.Details(id);

            // Assert
            result.Should().BeAssignableTo<IActionResult>();
        }

        [Fact]
        public async Task ClubsController_Index_Details_ReturnsSuccess()
        {
            // Arrange
            var id = 1;
            var club = A.Fake<Club>();
            A.CallTo(() => _clubService.GetClub(id)).Returns(club);
            
            // Act
            var result = await _controller.Index(id);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Details");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(club);
        }

        [Fact]
        public async Task ClubsController_Index_Details_ReturnsError()
        {
            // Arrange
            var id = 1;
            A.CallTo(() => _clubService.GetClub(id)).Returns(Task.FromResult<Club>(null));

            // Act
            var result = await _controller.Index(id);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }


        [Fact]
        public void ClubsController_CreateGet_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _controller.Create();

            // Assert
            result.Should().BeAssignableTo<IActionResult>();
        }

        //rever este teste pois não está muito bom
        [Fact]
        public void ClubsController_CreatePost_ReturnsSuccess()
        {
            //string userId = "fake_user_id";
            //Club clubToCreate = new Club { Name = "Test Club", ModalitiesIds = new List<int> { 1, 2, 3 } , UsersRoleClub = new List<UsersRoleClub> { new UsersRoleClub { Id = 1 } } };
            //var createdClub = new Club
            //{
            //    Id = 123,
            //    UsersRoleClub = new List<UsersRoleClub>
            //    {
            //        new UsersRoleClub { Id = 1, UserId = userId, RoleId = 50, JoinDate = DateTime.Now }
            //    }
            //};

            //// Set up the mock service to return the created club
            //A.CallTo(() => _clubService.CreateClub(clubToCreate, userId)).Returns(createdClub);

            //// Update the selected role for the user
            //await _userService.UpdateSelectedRole(userId, createdClub.UsersRoleClub!.First().Id);

            //// Act
            //var result = await _controller.Create(clubToCreate);

            //// Assert
            //result.Should().BeOfType<RedirectToActionResult>()
            //    .Which.ControllerName.Should().Be("MyClub");
            //result.As<RedirectToActionResult>().ActionName.Should().Be("Index");

            //A.CallTo(() => _clubService.CreateClub(clubToCreate, userId)).MustHaveHappenedOnceExactly();
            //A.CallTo(() => _userService.UpdateSelectedRole(userId, A<int>._)).MustHaveHappenedOnceExactly();

            // Arrange

            string userId = "fake_user_id";
            Club createdClub = new Club { Id = 123 };

            A.CallTo(() => _clubService.CreateClub(A<Club>._, userId)).Returns(createdClub);

            Club clubToCreate = new Club { Name = "Test Club", ModalitiesIds = new List<int> { 1, 2, 3 } };

            // Act
            var result = _controller.Create(clubToCreate);

            // Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }

        [Fact]
        public async Task ClubsController_Associate_ReturnsSuccess()
        {
            // Arrange
            var id = 1;
            var club = A.Fake<Club>();
            A.CallTo(() => _clubService.GetClub(id)).Returns(club);

            // Act
            var result = await _controller.Associate(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task ClubsController_Associate_ReturnsError()
        {
            // Arrange

            // Act
            var result = await _controller.Associate(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");

            // Arrange
            var id = 1;
            var club = A.Fake<Club>();
            A.CallTo(() => _clubService.GetClub(id)).Returns(Task.FromResult<Club>(null));

            // Act
            result = await _controller.Associate(id);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public void ClubsController_JoinGet_ReturnsSuccess()
        {
            // Arrange
            string code = "code";

            // Act
            var result =  _controller.Join(code);

            // Assert
            result.Should().BeAssignableTo<IActionResult>();
        }

        //rever este teste pois não está muito bom
        [Fact]
        public async Task ClubsController_JoinPost_ReturnsSuccess()
        {
            // Arrange
            var userId = "1";
            CodeClub codeClub = A.Fake<CodeClub>();

            A.CallTo(() => _clubService.UseCode(userId, codeClub)).ReturnsLazily((KeyValuePair<bool, string> p) => p);

            // Act
            var result = await _controller.Join(codeClub);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Join");

        }
    }
}

