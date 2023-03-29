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
using static SCManagement.Controllers.PlansController;

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
        public async Task PlansControllerTests_Templates_ReturnsSuccess()
        {
            // Arrange
            
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            var trainingPlan = new List<TrainingPlan>();
            var mealPlan = new List<MealPlan>();
            A.CallTo(() => _planService.GetTemplateTrainingPlans(A<string>._)).Returns(trainingPlan);
            A.CallTo(() => _planService.GetTemplateMealPlans(A<string>._)).Returns(mealPlan);


            // Act
            var result = await _controller.Templates();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public async Task PlansControllerTests_Templates_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.Templates();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        [Fact]
        public async Task PlansControllerTests_Templates_ReturnsNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlans(A<string>._)).Returns(Task.FromResult<IEnumerable<TrainingPlan>>(null));

            // Act
            var result = await _controller.Templates();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_AthleteTrainingPlans_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlans(A<string>._, A<string>._)).Returns(A.Fake<IEnumerable<TrainingPlan>>());

            // Act
            var result = await _controller.AthleteTrainingPlans("1");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public async Task PlansControllerTests_AthleteTrainingPlans_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

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
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlans(A<string>._, A<string>._)).Returns(Task.FromResult<IEnumerable<TrainingPlan>>(null));

            // Act
            var result = await _controller.AthleteTrainingPlans("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_AthleteMealPlans_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlans(A<string>._, A<string>._)).Returns(A.Fake<IEnumerable<MealPlan>>());

            // Act
            var result = await _controller.AthleteMealPlans("1");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public async Task PlansControllerTests_AthleteMealPlans_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

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
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
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
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
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
        public async Task PlansControllerTests_DeleteTrainingPlan_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

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
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
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
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
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
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan { TrainerId = "", IsTemplate = true });

            // Act
            var result = await _controller.DeleteTrainingPlan(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Templates");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteMealPlan_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan { TrainerId = "", IsTemplate = false });

            // Act
            var result = await _controller.DeleteMealPlan(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("AthleteMealPlans");
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
        public async Task PlansControllerTests_DeleteMealPlan_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

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
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
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
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
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
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan { TrainerId = "", IsTemplate = true });

            // Act
            var result = await _controller.DeleteMealPlan(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Templates");
        }

        [Fact]
        public async Task PlansControllerTests_ChooseTrainingTemplates_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlans(A<string>._)).Returns(A.Fake<List<TrainingPlan>>());

            // Act
            var result = await _controller.ChooseTrainingTemplates("1");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_ChooseTrainingTemplates_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.ChooseTrainingTemplates(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_ChooseTrainingTemplates_ReturnsIsNotClubTrainer()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.ChooseTrainingTemplates("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_ChooseTrainingTemplates_ReturnsTemplatesNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlans(A<string>._)).Returns(Task.FromResult<IEnumerable<TrainingPlan>>(null));

            // Act
            var result = await _controller.ChooseTrainingTemplates("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_ChooseMealTemplates_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlans(A<string>._)).Returns(A.Fake<List<MealPlan>>());

            // Act
            var result = await _controller.ChooseMealTemplates("1");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_ChooseMealTemplates_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.ChooseMealTemplates(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_ChooseMealTemplates_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.ChooseMealTemplates("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_ChooseMealTemplates_ReturnsTemplatesNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlans(A<string>._)).Returns(Task.FromResult<IEnumerable<MealPlan>>(null));

            // Act
            var result = await _controller.ChooseMealTemplates("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_TrainingDetails_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(false);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan { TrainerId = "", AthleteId = "" });

            // Act
            var result = await _controller.TrainingDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public async Task PlansControllerTests_TrainingDetails_ReturnsIsNotClubStaffAndIsNotClubAthlete()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.TrainingDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        [Fact]
        public async Task PlansControllerTests_TrainingDetails_ReturnsPlanNuLL()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(false);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(Task.FromResult<TrainingPlan>(null));

            // Act
            var result = await _controller.TrainingDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_TrainingDetails_ReturnsDiffIds()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(false);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan { TrainerId = "1", AthleteId = "2" });

            // Act
            var result = await _controller.TrainingDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_MealDetails_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(false);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan { TrainerId = "", AthleteId = "" });

            // Act
            var result = await _controller.MealDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public async Task PlansControllerTests_MealDetails_ReturnsIsNotClubStaffAndIsNotClubAthlete()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.MealDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        [Fact]
        public async Task PlansControllerTests_MealDetails_ReturnsPlanNuLL()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(false);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(Task.FromResult<MealPlan>(null));

            // Act
            var result = await _controller.MealDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_MealDetails_ReturnsDiffIds()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(false);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan { TrainerId = "1", AthleteId = "2" });

            // Act
            var result = await _controller.MealDetails(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateGoal_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateGoal("1");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateGoal_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateGoal("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateGoal_Post_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateGoal(new Goal { AthleteId = "1", TrainerId = "2", Name = "Meta 1",});

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlanTemplate_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateTrainingPlanTemplate();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlanTemplate_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateTrainingPlanTemplate();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlanTemplate_Post_ReturnsAddSessionsSuccess()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = true
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            
            // Act
            var result = await _controller.CreateTrainingPlanTemplate(trainingPlan, "Add sessions");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlanTemplate_Post_ReturnsCreateSuccess()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = true,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateTrainingPlanTemplate(trainingPlan, "Create");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Templates");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlanTemplate_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = true,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateTrainingPlanTemplate(trainingPlan, "Create");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlanTemplate_Post_ReturnsNotFound()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = true,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateTrainingPlanTemplate(trainingPlan, "aplicar");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");

        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_ReturnsSuccess()
        {
            // Arrange
            var template = new TrainingPlan
            {
                Name = "Treino 1",
                TrainerId = "",
                ModalityId = 1,
                IsTemplate = true,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlan(A<int>._)).Returns(template);

            // Act
            var result = await _controller.CreateTeamTrainingPlan(1,1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_ReturnsTeamIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.CreateTeamTrainingPlan(null,null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateTeamTrainingPlan(1, 1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_ReturnsTemplateNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlan(A<int>._)).Returns(Task.FromResult<TrainingPlan>(null));

            // Act
            var result = await _controller.CreateTeamTrainingPlan(1, 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_ReturnsUsersIdDiff()
        {
            // Arrange
            var template = new TrainingPlan
            {
                Name = "Treino 1",
                TrainerId = "2",
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlan(A<int>._)).Returns(template);

            // Act
            var result = await _controller.CreateTeamTrainingPlan(1, 1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_Post_ReturnsAddSessionsSuccess()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(new Team { Id = 1, Name = "Team 1", TrainerId = "", ClubId = 1, Athletes = new List<User>() { new User { Id = "1" } } });

            // Act
            var result = await _controller.CreateTeamTrainingPlan(trainingPlan, "Add sessions",null,1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_Post_ReturnsApplySuccess()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(new Team { Id = 1, Name = "Team 1", TrainerId = "", ClubId = 1, Athletes = new List<User>() { new User { Id = "1" } } });

            // Act
            var result = await _controller.CreateTeamTrainingPlan(trainingPlan, "Apply", "true", 1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TrainingZone");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_Post_ReturnsTeamNull()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(Task.FromResult<Team>(null));

            // Act
            var result = await _controller.CreateTeamTrainingPlan(trainingPlan, "Apply", "true", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_Post_ReturnsUsersIdsDiff()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(new Team { Id = 1, Name = "Team 1", TrainerId = "", ClubId = 1, Athletes = new List<User>() { new User { Id = "1" } } });

            // Act
            var result = await _controller.CreateTeamTrainingPlan(trainingPlan, "Apply", "true", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_Post_ReturnsTeamUsersIdsDiff()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(new Team { Id = 1, Name = "Team 1", TrainerId = "1", ClubId = 1, Athletes = new List<User>() { new User { Id = "1" } } });

            // Act
            var result = await _controller.CreateTeamTrainingPlan(trainingPlan, "Apply", "true", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(new Team { Id = 1, Name = "Team 1", TrainerId = "", ClubId = 1, Athletes = new List<User>() { new User { Id = "1" } } });

            // Act
            var result = await _controller.CreateTeamTrainingPlan(trainingPlan, "Add sessions", null, 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamTrainingPlan_Post_ReturnsNotFound()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(new Team { Id = 1, Name = "Team 1", TrainerId = "", ClubId = 1, Athletes = new List<User>() { new User { Id = "1" } } });

            // Act
            var result = await _controller.CreateTeamTrainingPlan(trainingPlan, "not apply", null, 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlan_ReturnsSuccess()
        {
            // Arrange
            var template = new TrainingPlan
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlan(A<int>._)).Returns(template);

            // Act
            var result = await _controller.CreateTrainingPlan("1", 1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlan_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateTrainingPlan("1", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlan_ReturnsTemplateNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlan(A<int>._)).Returns(Task.FromResult<TrainingPlan>(null));

            // Act
            var result = await _controller.CreateTrainingPlan("1", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlan_ReturnsUsersDiff()
        {
            // Arrange
            var template = new TrainingPlan
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlan(A<int>._)).Returns(template);

            // Act
            var result = await _controller.CreateTrainingPlan("1", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlan_Post_ReturnsAddSessionsSuccess()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateTrainingPlan(trainingPlan, "Add sessions", null);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlan_Post_ReturnsApplySuccess()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            
            // Act
            var result = await _controller.CreateTrainingPlan(trainingPlan, "Apply", "true");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TrainingZone");
        }
        
        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlan_Post_ReturnsUsersIdsDiff()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateTrainingPlan(trainingPlan, "Apply", "true");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlan_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        Id = 1,
                        TrainingPlanId = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                        Duration = 10,
                        Repetitions = 10,
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateTrainingPlan(trainingPlan, "Add sessions", null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTrainingPlan_Post_ReturnsNotFound()
        {
            // Arrange
            var trainingPlan = new CreateTrainingPlanModel
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        Id = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateTrainingPlan(trainingPlan, "not apply", null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlanTemplate_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateMealPlanTemplate();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlanTemplate_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateMealPlanTemplate();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlanTemplate_Post_ReturnsAddSessionsSuccess()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = true
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateMealPlanTemplate(mealPlan, "Add sessions");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlanTemplate_Post_ReturnsCreateSuccess()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = true,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateMealPlanTemplate(mealPlan, "Create");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Templates");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlanTemplate_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = true,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateMealPlanTemplate(mealPlan, "Create");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlanTemplate_Post_ReturnsNotFound()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = true,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateMealPlanTemplate(mealPlan, "aplicar");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");

        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_ReturnsSuccess()
        {
            // Arrange
            var template = new MealPlan
            {
                Name = "Treino 1",
                TrainerId = "",
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlan(A<int>._)).Returns(template);

            // Act
            var result = await _controller.CreateTeamMealPlan(1, 1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_ReturnsTeamIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.CreateTeamMealPlan(null, null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateTeamMealPlan(1, 1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_ReturnsTemplateNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlan(A<int>._)).Returns(Task.FromResult<MealPlan>(null));

            // Act
            var result = await _controller.CreateTeamMealPlan(1, 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_ReturnsUsersIdDiff()
        {
            // Arrange
            var template = new MealPlan
            {
                Name = "Treino 1",
                TrainerId = "",
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlan(A<int>._)).Returns(template);

            // Act
            var result = await _controller.CreateTeamMealPlan(1, 1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_Post_ReturnsAddSessionsSuccess()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = true,
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateTeamMealPlan(mealPlan, "Add sessions", null, 1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_Post_ReturnsApplySuccess()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = true,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(new Team { Id = 1, Name = "Team 1", TrainerId = "", ClubId = 1, Athletes = new List<User>() { new User { Id = "1" } } });

            // Act
            var result = await _controller.CreateTeamMealPlan(mealPlan, "Apply", "true", 1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TrainingZone");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_Post_ReturnsTeamNull()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(Task.FromResult<Team>(null));

            // Act
            var result = await _controller.CreateTeamMealPlan(mealPlan, "Apply", "true", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_Post_ReturnsUsersIdsDiff()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateTeamMealPlan(mealPlan, "Apply", "true", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_Post_ReturnsTeamUsersIdsDiff()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _teamService.GetTeam(A<int>._)).Returns(new Team { Id = 1, Name = "Team 1", TrainerId = "1", ClubId = 1, Athletes = new List<User>() { new User { Id = "1" } } });

            // Act
            var result = await _controller.CreateTeamMealPlan(mealPlan, "Apply", "true", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateTeamMealPlan(mealPlan, "Add sessions", null, 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateTeamMealPlan_Post_ReturnsNotFound()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateTeamMealPlan(mealPlan, "not apply", null, 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlan_ReturnsSuccess()
        {
            // Arrange
            var template = new MealPlan
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = true,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlan(A<int>._)).Returns(template);

            // Act
            var result = await _controller.CreateMealPlan("1", 1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlan_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateMealPlan("1", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlan_ReturnsTemplateNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlan(A<int>._)).Returns(Task.FromResult<MealPlan>(null));

            // Act
            var result = await _controller.CreateMealPlan("1", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlan_ReturnsUsersDiff()
        {
            // Arrange
            var template = new MealPlan
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = true,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlan(A<int>._)).Returns(template);

            // Act
            var result = await _controller.CreateMealPlan("1", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlan_Post_ReturnsAddSessionsSuccess()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateMealPlan(mealPlan, "Add sessions", null);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlan_Post_ReturnsApplySuccess()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateMealPlan(mealPlan, "Apply", "true");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("TrainingZone");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlan_Post_ReturnsUsersIdsDiff()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "2",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateMealPlan(mealPlan, "Apply", "true");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlan_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CreateMealPlan(mealPlan, "Add sessions", null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CreateMealPlan_Post_ReturnsNotFound()
        {
            // Arrange
            var mealPlan = new CreateMealPlanModel
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.CreateMealPlan(mealPlan, "not apply", null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan { TrainerId = "" });

            // Act
            var result = await _controller.EditTrainingPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.EditTrainingPlan(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.EditTrainingPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_ReturnsPlanNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(Task.FromResult<TrainingPlan>(null));

            // Act
            var result = await _controller.EditTrainingPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_ReturnsDiffIds()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan { TrainerId = "1" });

            // Act
            var result = await _controller.EditTrainingPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        
        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_Post_ReturnsEdit()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        Id = 1,
                        TrainingPlanId = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                        Repetitions = 10,
                        Duration = 10
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan 
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "", 
                IsTemplate = false, 
                TrainingPlanSessions = new List<TrainingPlanSession>() 
                { 
                    new TrainingPlanSession
                    {
                        Id = 1,
                        TrainingPlanId = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                        Repetitions = 10,
                        Duration = 10
                    } 
                } 
            });

            // Act
            var result = await _controller.EditTrainingPlan(trainingPlan, "Edit");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("AthleteTrainingPlans");
        }

        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_Post_ReturnsEditTemplate()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = true,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        Id = 1,
                        TrainingPlanId = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                        Repetitions = 10,
                        Duration = 10
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                IsTemplate = true,
                TrainingPlanSessions = new List<TrainingPlanSession>()
                {
                    new TrainingPlanSession
                    {
                        Id = 1,
                        TrainingPlanId = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                        Repetitions = 10,
                        Duration = 10
                    }
                }
            });

            // Act
            var result = await _controller.EditTrainingPlan(trainingPlan, "Edit");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Templates");
        }


        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_Post_ReturnsNotFound()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = true,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        Id = 1,
                        TrainingPlanId = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                        Repetitions = 10,
                        Duration = 10
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.EditTrainingPlan(trainingPlan, "not edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.EditTrainingPlan(trainingPlan, "Edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_Post_ReturnsUsersDiff()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "123",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        Id = 1,
                        TrainingPlanId = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                        Repetitions = 10,
                        Duration = 10
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.EditTrainingPlan(trainingPlan, "Edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_Post_ReturnsAddSessions()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.EditTrainingPlan(trainingPlan, "Add sessions");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_Post_ReturnsPlanNull()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        Id = 1,
                        TrainingPlanId = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                        Repetitions = 10,
                        Duration = 10
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(Task.FromResult<TrainingPlan>(null));

            // Act
            var result = await _controller.EditTrainingPlan(trainingPlan, "Edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_EditTrainingPlan_Post_ReturnsNotTheSameTrainer()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ModalityId = 1,
                IsTemplate = false,
                TrainingPlanSessions = new List<TrainingPlanSession>
                {
                    new TrainingPlanSession
                    {
                        Id = 1,
                        TrainingPlanId = 1,
                        ExerciseName= "Exercicio 1",
                        ExerciseDescription = "Aerobico",
                        Repetitions = 10,
                        Duration = 10
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTrainingPlan(A<int>._)).Returns(new TrainingPlan { TrainerId = "1"});

            // Act
            var result = await _controller.EditTrainingPlan(trainingPlan, "Edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }


        [Fact]
        public async Task PlansControllerTests_EditMealPlan_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan { TrainerId = "" });

            // Act
            var result = await _controller.EditMealPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public async Task PlansControllerTests_EditMealPlan_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.EditMealPlan(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        [Fact]
        public async Task PlansControllerTests_EditMealPlan_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.EditMealPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }
        [Fact]
        public async Task PlansControllerTests_EditMealPlan_ReturnsPlanNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(Task.FromResult<MealPlan>(null));

            // Act
            var result = await _controller.EditMealPlan(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_EditMealPlan_Post_ReturnsEdit()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.UtcNow.TimeOfDay,
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.UtcNow.TimeOfDay,
                    }
                }
            });

            // Act
            var result = await _controller.EditMealPlan(mealPlan, "Edit");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("AthleteMealPlans");
        }

        [Fact]
        public async Task PlansControllerTests_EditMealPlan_Post_ReturnsEditTemplate()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = true,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.UtcNow.TimeOfDay,
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                IsTemplate = true,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.UtcNow.TimeOfDay,
                    }
                }
            });

            // Act
            var result = await _controller.EditMealPlan(mealPlan, "Edit");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Templates");
        }


        [Fact]
        public async Task PlansControllerTests_EditMealPlan_Post_ReturnsNotFound()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.UtcNow.TimeOfDay,
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.EditMealPlan(mealPlan, "not edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_EditMealPlan_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.EditMealPlan(mealPlan, "Edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_EditMealPlan_Post_ReturnsUsersDiff()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "123",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.UtcNow.TimeOfDay,
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            
            // Act
            var result = await _controller.EditMealPlan(mealPlan, "Edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_EditMealPlan_Post_ReturnsAddSessions()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.EditMealPlan(mealPlan, "Add sessions");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_EditMealPlan_Post_ReturnsPlanNull()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.UtcNow.TimeOfDay,
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(Task.FromResult<MealPlan>(null));

            // Act
            var result = await _controller.EditMealPlan(mealPlan, "Edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_EditMealPlan_Post_ReturnsNotTheSameTrainer()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Id = 1,
                Name = "Treino 1",
                AthleteId = "1",
                TrainerId = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        Id = 1,
                        MealPlanId = 1,
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.UtcNow.TimeOfDay,
                    }
                }
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetMealPlan(A<int>._)).Returns(new MealPlan { TrainerId = "1" });

            // Act
            var result = await _controller.EditMealPlan(mealPlan, "Edit");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_EditGoalGoal_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoal(A<int>._)).Returns(new Goal { AthleteId = "1", TrainerId = "", Name = "Meta 1",isCompleted = false });

            // Act
            var result = await _controller.EditGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_EditGoalGoal_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.EditGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_EditGoalGoal_ReturnsTrainerIdDiff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoal(A<int>._)).Returns(new Goal { AthleteId = "1", TrainerId = "123", Name = "Meta 1", isCompleted = false });

            // Act
            var result = await _controller.EditGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_EditGoalGoal_ReturnsGoalNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoal(A<int>._)).Returns(Task.FromResult<Goal>(null));

            // Act
            var result = await _controller.EditGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_EditGoal_Post_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);

            // Act
            var result = await _controller.EditGoal(new Goal { AthleteId = "1", TrainerId = "", Name = "Meta 1", });

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("GoalsList");
        }

        [Fact]
        public async Task PlansControllerTests_EditGoal_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.EditGoal(new Goal { AthleteId = "1", TrainerId = "", Name = "Meta 1", });

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_EditGoal_Post_ReturnsTrainerIdDiff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.EditGoal(new Goal { AthleteId = "1", TrainerId = "1234123", Name = "Meta 1", });

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteGoal_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoal(A<int>._)).Returns(new Goal { AthleteId = "1", TrainerId = "", Name = "Meta 1", isCompleted = false });

            // Act
            var result = await _controller.DeleteGoal(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("GoalsList");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteGoal_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.DeleteGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteGoal_ReturnsGoalNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoal(A<int>._)).Returns(Task.FromResult<Goal>(null));

            // Act
            var result = await _controller.DeleteGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_DeleteGoal_ReturnsTrainerIdDiff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoal(A<int>._)).Returns(new Goal { AthleteId = "1", TrainerId = "23452345", Name = "Meta 1", isCompleted = false });

            // Act
            var result = await _controller.DeleteGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CompleteGoal_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoal(A<int>._)).Returns(new Goal { AthleteId = "", TrainerId = "", Name = "Meta 1", isCompleted = false });

            // Act
            var result = await _controller.CompleteGoal(1);

            // Assert
            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task PlansControllerTests_CompleteGoal_ReturnsIsNotClubAthlete()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.CompleteGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_CompleteGoal_ReturnsGoalNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsAtleteInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoal(A<int>._)).Returns(Task.FromResult<Goal>(null));

            // Act
            var result = await _controller.CompleteGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_CompleteGoal_ReturnsTrainerIdDiff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoal(A<int>._)).Returns(new Goal { AthleteId = "2345234", TrainerId = "dfg", Name = "Meta 1", isCompleted = false });

            // Act
            var result = await _controller.CompleteGoal(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_GoalsList_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoals(A<string>._, A<string>._)).Returns(A.Fake<List<Goal>>());

            // Act
            var result = await _controller.GoalsList("1");

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_GoalsList_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.GoalsList("1");

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_GoalsList_ReturnsGoalsNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetGoals(A<string>._, A<string>._)).Returns(Task.FromResult<IEnumerable<Goal>>(null));

            // Act
            var result = await _controller.GoalsList("1");


            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_ChooseTrainingTeamTemplates_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlans(A<string>._)).Returns(A.Fake<List<TrainingPlan>>());

            // Act
            var result = await _controller.ChooseTrainingTeamTemplates(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_ChooseTrainingTeamTemplates_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.ChooseTrainingTeamTemplates(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_ChooseTrainingTeamTemplates_ReturnsTemplateNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateTrainingPlans(A<string>._)).Returns(Task.FromResult<IEnumerable<TrainingPlan>>(null));

            // Act
            var result = await _controller.ChooseTrainingTeamTemplates(1);


            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task PlansControllerTests_ChooseMealTeamTemplates_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlans(A<string>._)).Returns(A.Fake<List<MealPlan>>());

            // Act
            var result = await _controller.ChooseMealTeamTemplates(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task PlansControllerTests_ChooseMealTeamTemplates_ReturnsIsNotClubStaff()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(false);

            // Act
            var result = await _controller.ChooseMealTeamTemplates(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task PlansControllerTests_ChooseMealTeamTemplates_ReturnsTemplateNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _userService.IsStaffInAnyClub(A<string>._)).Returns(true);
            A.CallTo(() => _planService.GetTemplateMealPlans(A<string>._)).Returns(Task.FromResult<IEnumerable<MealPlan>>(null));

            // Act
            var result = await _controller.ChooseMealTeamTemplates(1);


            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
    }
}
