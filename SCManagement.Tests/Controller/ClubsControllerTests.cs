using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Controllers;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PaymentService;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.UserService;
using static SCManagement.Controllers.ClubsController;

namespace SCManagement.Tests.Controller
{
    public class ClubsControllerTests
    {
        private readonly ClubsController _controller;
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;

        public ClubsControllerTests()
        {
            _userManager = A.Fake<UserManager<User>>();
            _clubService = A.Fake<IClubService>();
            _userService = A.Fake<IUserService>();
            _paymentService = A.Fake<IPaymentService>();

            //SUT (system under test)
            _controller = new ClubsController(_userManager, _clubService, _userService, _paymentService);
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
            var club = A.Fake<Club>();
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);

            // Act
            var result = _controller.Details(1);

            // Assert
            result.Should().BeAssignableTo<IActionResult>();
        }

        [Fact]
        public async Task ClubsController_Index_Details_ReturnsSuccess()
        {
            // Arrange
            var club = A.Fake<Club>();
            var ClubTranslations = new List<ClubTranslations>() { A.Fake<ClubTranslations>(), A.Fake<ClubTranslations>() };
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);
            A.CallTo(() => _clubService.GetClubTranslations(A<int>._)).Returns(ClubTranslations);
            A.CallTo(() => _clubService.IsClubPartner(A<string>._ ,A<int>._)).Returns(false);

            // Act
            var result = await _controller.Index(1);
            
            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Details");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(club);
        }

        [Fact]
        public async Task ClubsController_Index_Details_ReturnsIsPartnerSuccess()
        {
            // Arrange
            var club = A.Fake<Club>();
            var ClubTranslations = new List<ClubTranslations>() { A.Fake<ClubTranslations>(), A.Fake<ClubTranslations>() };
            var sub = A.Fake<Subscription>();
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);
            A.CallTo(() => _clubService.GetClubTranslations(A<int>._)).Returns(ClubTranslations);
            A.CallTo(() => _clubService.IsClubPartner(A<string>._, A<int>._)).Returns(true);
            A.CallTo(() => _paymentService.GetMembershipSubscription(A<string>._, A<int>._)).Returns(sub);

            // Act
            var result = await _controller.Index(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Details");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(club);
        }

        [Fact]
        public async Task ClubsController_Index_Details_ReturnsClubNull()
        {
            // Arrange
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(Task.FromResult<Club>(null));

            // Act
            var result = await _controller.Index(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }


        [Fact]
        public async Task ClubsController_CreateGet_ReturnsSuccess()
        {
            // Arrange
            
            // Act
            var result = await _controller.Create(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        
        [Fact]
        public async Task ClubsController_Create_Post_ReturnsSuccess()
        {
            // Arrange
            var createdClub = new Club
            {
                Id = 123,
                UsersRoleClub = new List<UsersRoleClub>() { new UsersRoleClub { Id = 1} },
                
            };
            var clubToCreate = new CreateClubModel { Name = "Test Club", ModalitiesIds = new List<int> { 1, 2, 3 }, PlanId = 1 };
            var clubSubProducts = new List<Product>() { new Product { Id = 1 } };
            A.CallTo(() => _clubService.CreateClub(A<Club>._, A<string>._)).Returns(createdClub);
            A.CallTo(() => _paymentService.GetClubSubscriptionPlans()).Returns(clubSubProducts);

            // Act
            var result = await _controller.Create(clubToCreate);
            
            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task ClubsController_Create_Post_ReturnsView()
        {
            // Arrange
            var createdClub = new Club
            {
                Id = 123,
                UsersRoleClub = new List<UsersRoleClub>() { new UsersRoleClub { Id = 1 } },

            };
            var clubToCreate = new CreateClubModel { Name = "Test Club", ModalitiesIds = new List<int> { 1, 2, 3 }, PlanId = 1 };
            var clubSubProducts = new List<Product>() { new Product { Id = 3 } };
            A.CallTo(() => _clubService.CreateClub(A<Club>._, A<string>._)).Returns(createdClub);
            A.CallTo(() => _paymentService.GetClubSubscriptionPlans()).Returns(clubSubProducts);

            // Act
            var result = await _controller.Create(clubToCreate);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task ClubsController_Associate_ReturnsSuccess()
        {
            // Arrange
            var club = A.Fake<Club>();
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);
            A.CallTo(() => _clubService.IsClubMember(A<string>._,A<int>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubPartner(A<string>._,A<int>._)).Returns(true);

            // Act
            var result = await _controller.Associate(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task ClubsController_Associate_ReturnsIsClubMember()
        {
            // Arrange
            var club = A.Fake<Club>();
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(club);
            A.CallTo(() => _clubService.IsClubMember(A<string>._, A<int>._)).Returns(true);
            A.CallTo(() => _clubService.IsClubPartner(A<string>._, A<int>._)).Returns(false);

            // Act
            var result = await _controller.Associate(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
            result.Should().BeOfType<RedirectToActionResult>().Which.ControllerName.Should().Be("Subscription");
        }

        [Fact]
        public async Task ClubsController_Associate_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _controller.Associate(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }


        [Fact]
        public async Task ClubsController_Associate_ReturnsClubNull()
        {
            // Arrange
            var club = A.Fake<Club>();
            A.CallTo(() => _clubService.GetClub(A<int>._)).Returns(Task.FromResult<Club>(null));
            
            // Act
            var result = await _controller.Associate(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
        
        [Fact]
        public void ClubsController_JoinGet_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result =  _controller.Join("code");

            // Assert
            result.Should().BeAssignableTo<IActionResult>();
        }

        //rever este teste pois não está muito bom
        [Fact]
        public async Task ClubsController_JoinPost_ReturnsSuccess()
        {
            // Arrange
            var codeClub = A.Fake<CodeClub>();
            var joined = new KeyValuePair<bool, string>(true, "Success");
            A.CallTo(() => _clubService.UseCode(A<string>._, A<CodeClub>._)).Returns(joined);

            // Act
            var result = await _controller.Join(codeClub);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task ClubsController_JoinPost_ReturnsFail()
        {
            // Arrange
            var codeClub = A.Fake<CodeClub>();
            var joined = new KeyValuePair<bool, string>(false, "Code_NotFound");
            A.CallTo(() => _clubService.UseCode(A<string>._, A<CodeClub>._)).Returns(joined);

            // Act
            var result = await _controller.Join(codeClub);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Join");
        }

        [Fact]
        public async Task ClubsController_CoordsMarkers_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _controller.CoordsMarkers();

            // Assert
            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task ClubsController_SearchNameClubs_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _controller.SearchNameClubs("Benfica");

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_PartialSearchClub");
        }

        [Fact]
        public async Task ClubsController_Plans_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _controller.Plans();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }
}

