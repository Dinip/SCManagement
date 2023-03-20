using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Controllers;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PlansService;
using SCManagement.Services.PlansService.Models;
using SCManagement.Services.TeamService;
using SCManagement.Services.UserService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCManagement.Tests.Controller
{
    public class PlansControllerTests
    {
        private readonly PlansController _controller;
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;
        private readonly IPlanService _planService;
        private readonly ITeamService _teamService;

        public PlansControllerTests()
        {
            _userManager = A.Fake<UserManager<User>>();
            _clubService = A.Fake<IClubService>();
            _userService = A.Fake<IUserService>();
            _planService = A.Fake<IPlanService>();
            _teamService = A.Fake<ITeamService>();

            //SUT (system under test)
            _controller = new PlansController(_userManager, _clubService, _userService, _planService, _teamService);
        }

        [Fact]
        public async Task PlansControllerTests_TrainingTemplates_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlans(A<string>._)).Returns(A.Fake<IEnumerable<TrainingPlan>>());

            // Act
            var result = await _controller.TrainingTemplates();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("TrainingPlans");
        }
        [Fact]
        public async Task PlansControllerTests_TrainingTemplates_ReturnsIsNotClubTrainer()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.TrainingTemplates();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        [Fact]
        public async Task PlansControllerTests_TrainingTemplates_ReturnsNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlans(A<string>._)).Returns(Task.FromResult<IEnumerable<TrainingPlan>>(null));

            // Act
            var result = await _controller.TrainingTemplates();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_AthleteTrainingPlans_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlans(A<string>._, A<string>._)).Returns(A.Fake<IEnumerable<TrainingPlan>>());

            // Act
            var result = await _controller.AthleteTrainingPlans("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("TrainingPlans");
        }
        [Fact]
        public async Task PlansControllerTests_AthleteTrainingPlans_ReturnsIsNotClubTrainer()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.AthleteTrainingPlans("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        [Fact]
        public async Task PlansControllerTests_AthleteTrainingPlans_ReturnsNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlans(A<string>._, A<string>._)).Returns(Task.FromResult<IEnumerable<TrainingPlan>>(null));

            // Act
            var result = await _controller.AthleteTrainingPlans("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_MealTemplates_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlans(A<string>._)).Returns(A.Fake<IEnumerable<MealPlan>>());

            // Act
            var result = await _controller.MealTemplates();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("MealPlans");
        }
        [Fact]
        public async Task PlansControllerTests_MealTemplates_ReturnsIsNotClubTrainer()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.MealTemplates();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        [Fact]
        public async Task PlansControllerTests_MealTemplates_ReturnsNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlans(A<string>._)).Returns(Task.FromResult<IEnumerable<MealPlan>>(null));

            // Act
            var result = await _controller.MealTemplates();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_AthleteMealPlans_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlans(A<string>._, A<string>._)).Returns(A.Fake<IEnumerable<MealPlan>>());

            // Act
            var result = await _controller.AthleteMealPlans("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("MealPlans");
        }
        [Fact]
        public async Task PlansControllerTests_AthleteMealPlans_ReturnsIsNotClubTrainer()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.AthleteMealPlans("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        [Fact]
        public async Task PlansControllerTests_AthleteMealPlans_ReturnsNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlans(A<string>._, A<string>._)).Returns(Task.FromResult<IEnumerable<MealPlan>>(null));

            // Act
            var result = await _controller.AthleteMealPlans("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteTrainingPlan_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan { TrainerId = "", IsTemplate = false });
            
            // Act
            var result = await _controller.DeleteTrainingPlan(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("AthleteTrainingPlans");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteTrainingPlan_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.DeleteTrainingPlan(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_DeleteTrainingPlan_ReturnsIsNotClubTrainer()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.DeleteTrainingPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteTrainingPlan_ReturnsPlanNotFound()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(Task.FromResult<TrainingPlan>(null));

            // Act
            var result = await _controller.DeleteTrainingPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteTrainingPlan_ReturnsDiffIds()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan { TrainerId = "1" });

            // Act
            var result = await _controller.DeleteTrainingPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteTrainingPlan_ReturnsTemplateSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan { TrainerId = "", IsTemplate = true });

            // Act
            var result = await _controller.DeleteTrainingPlan(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TrainingTemplates");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteMealPlan_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan { TrainerId = "", IsTemplate = false });

            // Act
            var result = await _controller.DeleteMealPlan(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("AthleteTrainingPlans");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteMealPlan_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.DeleteMealPlan(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_DeleteMealPlan_ReturnsIsNotClubTrainer()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _controller.DeleteMealPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteMealPlan_ReturnsPlanNotFound()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(Task.FromResult<MealPlan>(null));

            // Act
            var result = await _controller.DeleteMealPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteMealPlan_ReturnsDiffIds()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan { TrainerId = "1" });

            // Act
            var result = await _controller.DeleteMealPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteMealPlan_ReturnsTemplateSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _clubService.IsClubTrainer(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan { TrainerId = "", IsTemplate = true });

            // Act
            var result = await _controller.DeleteMealPlan(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TrainingTemplates");
        }
    }
}
