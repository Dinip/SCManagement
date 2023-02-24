using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Controllers;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.UserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCManagement.Tests.Controller
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UserControllerTests()
        {
            _userManager = A.Fake<UserManager<User>>();
            _userService = A.Fake<IUserService>();

            //SUT (system under test)
            _controller = new UserController(_userService, _userManager);
        }

        [Fact]
        public async Task UserController_Index_ReturnsUsersRoleClubNull()
        {
            // Arrange
            var user = A.Fake<User>();
            A.CallTo(() => _userService.GetUserWithRoles(A<string>._)).Returns(user);

            // Act
            var result = await _controller.UpdateSelectedRole(1);
            
            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
            result.Should().BeOfType<RedirectToActionResult>().Which.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task UserController_Index_ReturnsSuccess()
        {
            // Arrange
            var user = new User { Id = "Test 1", FirstName = "Tester", LastName = "1", Email = "a@gmail.com", UserName = "Tester 1", UsersRoleClub = new List<UsersRoleClub>() { new UsersRoleClub { Id = 1, UserId = "Test 1", RoleId = 40 } } };
            A.CallTo(() => _userService.GetUserWithRoles(A<string>._)).Returns(user);
            A.CallTo(() => _userService.UpdateSelectedRole(A<string>._, A<int>._)).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _controller.UpdateSelectedRole(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
            result.Should().BeOfType<RedirectToActionResult>().Which.ControllerName.Should().Be("MyClub");
        }
    }
}
