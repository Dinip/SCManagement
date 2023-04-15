using Microsoft.AspNetCore.Identity;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.EventService;
using SCManagement.Services.PaymentService;
using SCManagement.Services.UserService;
using FakeItEasy;
using FluentAssertions;
using SCManagement.Controllers;
using Microsoft.AspNetCore.Mvc;
using static SCManagement.Controllers.EventsController;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.TranslationService;
using System.Text.Json;
using SCManagement.Services.NotificationService;

namespace SCManagement.Tests.Controller {
    public class EventsControllerTests {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IPaymentService _paymentService;
        private readonly ITranslationService _translationService;
        private readonly EventsController _eventsController;
        private readonly INotificationService _notificationService;

        public EventsControllerTests()
        {
            _eventService = A.Fake<IEventService>();
            _userService = A.Fake<IUserService>();
            _userManager = A.Fake<UserManager<User>>();
            _clubService = A.Fake<IClubService>();
            _paymentService = A.Fake<IPaymentService>();
            _translationService = A.Fake<ITranslationService>();
            _notificationService = A.Fake<INotificationService>();

            //SUT (system under test)
            _eventsController = new EventsController(_eventService, _userService, _userManager, _clubService, _paymentService, _translationService, _notificationService);
        }

        [Fact]
        public async Task EventsController_Index_ReturnsSuccess()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            var events = new List<Event> { new Event { Id = 1, } };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);

            // Act
            var result = await _eventsController.Index(null);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EventsController_Index_ReturnsRoleNull()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            var events = new List<Event> { new Event { Id = 1, } };
            var clubEvents = new List<Event> { new Event { Id = 2, ClubId = 1 } };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(Task.FromResult<UsersRoleClub>(null));

            // Act
            var result = await _eventsController.Index(null);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EventsController_Details_ReturnsSuccess()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            var events = new Event { Id = 1, ClubId = 1, IsPublic = true, EventTranslations = new List<EventTranslation> { new EventTranslation { EventId = 1, Language = "en", Value = "test" } } };
            var enroll = new EventEnroll { Id = 1, EventId = 1, UserId = "1" };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(events);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(enroll);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.Details(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("EventDetails");
        }

        [Fact]
        public async Task EventsController_Details_ReturnsEnrollNull()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            var events = new Event { Id = 1, ClubId = 1, IsPublic = true, EventTranslations = new List<EventTranslation> { new EventTranslation { EventId = 1, Language = "en", Value = "test" } } };
            var enroll = new EventEnroll { Id = 1, EventId = 1, UserId = "1" };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(events);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(Task.FromResult<EventEnroll>(null));
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.Details(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("EventDetails");
        }

        [Fact]
        public async Task EventsController_Details_ReturnsRoleNotExist()
        {
            // Arrange
            var events = new Event { Id = 1, IsPublic = false, ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(A.Fake<UsersRoleClub>());
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(events);

            // Act
            var result = await _eventsController.Details(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_Details_ReturnsEventNull()
        {
            // Arrange
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));

            // Act
            var result = await _eventsController.Details(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_Details_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _eventsController.Details(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_Details_ReturnsRoleIdDiffEventClubId()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            var events = new Event { Id = 1, IsPublic = false, ClubId = 2 };
            var enroll = new EventEnroll { Id = 1, EventId = 1, UserId = "1" };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(events);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(enroll);

            // Act
            var result = await _eventsController.Details(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_Create_ReturnsSuccess()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.Create();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EventsController_Create_ReturnsRoleNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(Task.FromResult<UsersRoleClub>(null));

            // Act
            var result = await _eventsController.Create();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_Create_ReturnsIsNotClubStaff()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _eventsController.Create();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_Create_Post_ReturnsSuccess()
        {
            // Arrange
            var even = new EventModel
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now.AddDays(10),
                EnrollLimitDate = DateTime.Now.AddDays(5),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true,
                LocationString = "{\"CoordinateY\":38.5266275,\"CoordinateX\":-8.8677658,\"AddressString\":\"Rua Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal\"}",
                EventTranslationsName = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Name",
                    }
                },
                EventTranslationsDetails = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Details",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Details",
                    }
                },
            };
            var events = new Event { Id = 1, EventTranslations = new List<EventTranslation>() };
            var role = new UsersRoleClub { ClubId = 1 };
            var address = new Address
            {
                CoordinateY = 38.5225957,
                CoordinateX = -8.8390330,
                AddressString = "R Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal"
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.CreateEvent(A<Event>._)).Returns(events);
            A.CallTo(() => _paymentService.ClubHasValidKey(A<int>._)).Returns(true);
            A.CallTo(() => _eventService.CreateEventAddress(A<Address>._)).Returns(address);

            // Act
            var result = await _eventsController.Create(even);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task EventsController_Create_Post_ReturnsErrorDate()
        {
            // Arrange
            var even = new EventModel
            {
                Id = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10),
                EnrollLimitDate = DateTime.Now.AddDays(5),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true,
                LocationString = "{\"CoordinateY\":38.5266275,\"CoordinateX\":-8.8677658,\"AddressString\":\"Rua Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal\"}",
                EventTranslationsName = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Name",
                    }
                },
                EventTranslationsDetails = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Details",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Details",
                    }
                },
            };
            var events = new Event { Id = 1, EventTranslations = new List<EventTranslation>() };
            var role = new UsersRoleClub { ClubId = 1 };
            var address = new Address
            {
                CoordinateY = 38.5225957,
                CoordinateX = -8.8390330,
                AddressString = "R Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal"
            };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.CreateEvent(A<Event>._)).Returns(events);
            A.CallTo(() => _paymentService.ClubHasValidKey(A<int>._)).Returns(true);
            A.CallTo(() => _eventService.CreateEventAddress(A<Address>._)).Returns(address);

            // Act
            var result = await _eventsController.Create(even);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_InvalidInput");
        }

        [Fact]
        public async Task EventsController_Create_Post_ReturnsIsNotClubStaff()
        {
            // Arrange
            var even = new EventModel
            {
                Id = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true,
            };
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _eventsController.Create(even);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }


        [Fact]
        public async Task EventsController_Edit_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _eventsController.Edit(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_Edit_ReturnsEventNull()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));

            // Act
            var result = await _eventsController.Edit(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_Edit_ReturnsIsNotClubStaff()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            var e = new Event { Id = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);

            // Act
            var result = await _eventsController.Edit(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_Edit_Post_ReturnsSuccess()
        {
            // Arrange
            var a = new List<EventTranslation>()
            {
                new EventTranslation
                {
                    EventId = 1,
                    Value = "Olá",
                    Language = "pt-PT",
                    Atribute = "Name",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "",
                    Language = "en-US",
                    Atribute = "Name",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "Olá",
                    Language = "pt-PT",
                    Atribute = "Details",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "",
                    Language = "en-US",
                    Atribute = "Details",
                }
            };

            var even = new EventModel
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                Route = "Lisboa",
                HaveRoute = true,
                EventTranslationsName = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Name",
                    }
                },
                EventTranslationsDetails = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Details",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Details",
                    }
                },
            };

            var eventAUX = new Event
            {
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true
            };

            even.EventAux = JsonSerializer.Serialize(eventAUX);

            var role = new UsersRoleClub { ClubId = 1 };
            var e = new Event { Id = 1, ClubId = 1, EventTranslations = a, LocationId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);

            // Act
            var result = await _eventsController.Edit(1, even);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task EventsController_Edit_Post_ReturnsUpdateLocation()
        {
            // Arrange
            var a = new List<EventTranslation>()
            {
                new EventTranslation
                {
                    EventId = 1,
                    Value = "Olá",
                    Language = "pt-PT",
                    Atribute = "Name",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "",
                    Language = "en-US",
                    Atribute = "Name",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "Olá",
                    Language = "pt-PT",
                    Atribute = "Details",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "",
                    Language = "en-US",
                    Atribute = "Details",
                }
            };

            var even = new EventModel
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true,
                LocationString = "{\"CoordinateY\":38.5266275,\"CoordinateX\":-8.8677658,\"AddressString\":\"Rua Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal\"}",
                EventTranslationsName = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Name",
                    }
                },
                EventTranslationsDetails = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Details",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Details",
                    }
                },
            };

            var eventAUX = new Event
            {
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true
            };

            even.EventAux = JsonSerializer.Serialize(eventAUX);
            var address = new Address
            {
                CoordinateY = 38.5225957,
                CoordinateX = -8.8390330,
                AddressString = "R Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal"
            };
            var role = new UsersRoleClub { ClubId = 1 };
            var e = new Event { Id = 1, ClubId = 1, EventTranslations = a, LocationId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.UpdateEventAddress(A<int>._, A<Address>._)).Returns(address);

            // Act
            var result = await _eventsController.Edit(1, even);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task EventsController_Edit_Post_ReturnsCreateLocation()
        {
            // Arrange
            var a = new List<EventTranslation>()
            {
                new EventTranslation
                {
                    EventId = 1,
                    Value = "Olá",
                    Language = "pt-PT",
                    Atribute = "Name",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "",
                    Language = "en-US",
                    Atribute = "Name",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "Olá",
                    Language = "pt-PT",
                    Atribute = "Details",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "",
                    Language = "en-US",
                    Atribute = "Details",
                }
            };

            var eventAUX = new Event
            {
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true
            };
            
            var even = new EventModel
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true,
                LocationString = "{\"CoordinateY\":38.5266275,\"CoordinateX\":-8.8677658,\"AddressString\":\"Rua Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal\"}",
                EventTranslationsName = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Name",
                    }
                },
                EventTranslationsDetails = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Details",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Details",
                    }
                },
            };

            even.EventAux = JsonSerializer.Serialize(eventAUX);


            var address = new Address
            {
                CoordinateY = 38.5225957,
                CoordinateX = -8.8390330,
                AddressString = "R Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal"
            };
            var role = new UsersRoleClub { ClubId = 1 };
            var e = new Event { Id = 1, ClubId = 1, EventTranslations = a, LocationId = null };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.UpdateEventAddress(A<int>._, A<Address>._)).Returns(address);

            // Act
            var result = await _eventsController.Edit(1, even);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task EventsController_Edit_Post_ReturnsIdDiff()
        {
            // Arrange
            var even = new EventModel
            {
                Id = 2,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true,
            };

            // Act
            var result = await _eventsController.Edit(1, even);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_Edit_Post_ReturnsDateError()
        {
            // Arrange
            var a = new List<EventTranslation>()
            {
                new EventTranslation
                {
                    EventId = 1,
                    Value = "Olá",
                    Language = "pt-PT",
                    Atribute = "Name",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "",
                    Language = "en-US",
                    Atribute = "Name",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "Olá",
                    Language = "pt-PT",
                    Atribute = "Details",
                },
                new EventTranslation
                {
                    EventId = 1,
                    Value = "",
                    Language = "en-US",
                    Atribute = "Details",
                }
            };

            var even = new EventModel
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                Route = "Lisboa",
                HaveRoute = true,
                EventTranslationsName = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Name",
                    }
                },
                EventTranslationsDetails = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "Olá",
                        Language = "pt-PT",
                        Atribute = "Details",
                    },
                    new EventTranslation
                    {
                        EventId = 1,
                        Value = "",
                        Language = "en-US",
                        Atribute = "Details",
                    }
                },
            };

            var eventAUX = new Event
            {
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true
            };

            even.EventAux = JsonSerializer.Serialize(eventAUX);

            var role = new UsersRoleClub { ClubId = 1 };
            var e = new Event { Id = 1, ClubId = 1, EventTranslations = a, LocationId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);

            // Act
            even.EndDate = even.StartDate.AddDays(-1);

            var result = await _eventsController.Edit(1, even);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_InvalidInput");
        }

        [Fact]
        public async Task EventsController_Edit_Post_ReturnsIsNotClubStaff()
        {
            var eventAUX = new Event
            {
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true
            };

            // Arrange
            var even = new EventModel
            {
                Id = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true,
                Route = "-8.898746,38.525216;-8.898611,38.525169;-8.898556,38.525053;-8.898349,38.523619;-8.898253,38.523312;-8.898173,38.523244;-8.898062,38.523157;-8.897485,38.523247;-8.894789,38.523485;-8.8948,38.523557"
            };
            even.EventAux = JsonSerializer.Serialize(eventAUX);

            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _eventsController.Edit(1, even);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_Edit_Post_ReturnsEventNull()
        {
            // Arrange
            var even = new EventModel
            {
                Id = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true,
                Route = "-8.898746,38.525216;-8.898611,38.525169;-8.898556,38.525053;-8.898349,38.523619;-8.898253,38.523312;-8.898173,38.523244;-8.898062,38.523157;-8.897485,38.523247;-8.894789,38.523485;-8.8948,38.523557"
            };
            var role = new UsersRoleClub { ClubId = 1 };

            var eventAUX = new Event
            {
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5),
                EnrollLimitDate = DateTime.Now.AddDays(1),
                IsPublic = true,
                Fee = 10,
                HaveRoute = true
            };
            even.EventAux = JsonSerializer.Serialize(eventAUX);

            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));

            // Act
            var result = await _eventsController.Edit(1, even);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_Delete_ReturnsSuccess()
        {
            // Arrange
            var e = new Event { Id = 1 };
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.Delete(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task EventsController_Delete_ReturnsIsNotClubStaff()
        {
            // Arrange
            var e = new Event { Id = 1 };
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(false);

            // Act
            var result = await _eventsController.Delete(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_Delete_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _eventsController.Delete(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_Delete_ReturnsEventNull()
        {
            // Arrange
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));

            // Act
            var result = await _eventsController.Delete(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_EventEnrollment_ReturnsSuccess()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, MaxEventEnrolls = 10, EnrollLimitDate = DateTime.Now.AddDays(1) };
            var role = new UsersRoleClub { ClubId = 1 };
            var createEnroll = new EventEnroll { EventId = 1, UserId = "1" };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(Task.FromResult<EventEnroll>(null));
            A.CallTo(() => _eventService.CreateEventEnroll(A<EventEnroll>._)).Returns(createEnroll);
            A.CallTo(() => _paymentService.CreateEventPayment(A<EventEnroll>._)).Returns(Task.FromResult<Payment>(null));

            // Act
            var result = await _eventsController.EventEnrollment(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Details");
        }

        [Fact]
        public async Task EventsController_EventEnrollment_ReturnsEventToEnrollnULL()
        {
            // Arrange
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));

            // Act
            var result = await _eventsController.EventEnrollment(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_EventEnrollment_ReturnsIsNotPublic()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = false, ClubId = 1 };
            var role = new UsersRoleClub { ClubId = 2 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);

            // Act
            var result = await _eventsController.EventEnrollment(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_EventEnrollment_ReturnsUserAlreadyEnrolled()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true };
            var role = new UsersRoleClub { ClubId = 1 };
            var enroll = new EventEnroll { EventId = 1, UserId = "1" };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(enroll);

            // Act
            var result = await _eventsController.EventEnrollment(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_AlreadyEnrolled");
        }

        [Fact]
        public async Task EventsController_EventEnrollment_ReturnsPay()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, MaxEventEnrolls = 10, EnrollLimitDate = DateTime.Now.AddDays(1) };
            var role = new UsersRoleClub { ClubId = 1 };
            var createEnroll = new EventEnroll { EventId = 1, UserId = "1" };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(Task.FromResult<EventEnroll>(null));
            A.CallTo(() => _eventService.CreateEventEnroll(A<EventEnroll>._)).Returns(createEnroll);
            A.CallTo(() => _paymentService.CreateEventPayment(A<EventEnroll>._)).Returns(A.Fake<Payment>());

            // Act
            var result = await _eventsController.EventEnrollment(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
            result.Should().BeOfType<RedirectToActionResult>().Which.ControllerName.Should().Be("Payment");
        }

        [Fact]
        public async Task EventsController_EventEnrollment_ReturnsMaxEventEnrolls()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, MaxEventEnrolls = 10, EnrollLimitDate = DateTime.Now.AddDays(1) };
            var role = new UsersRoleClub { ClubId = 1 };
            var createEnroll = new EventEnroll { EventId = 1, UserId = "1" };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(Task.FromResult<EventEnroll>(null));
            A.CallTo(() => _eventService.CreateEventEnroll(A<EventEnroll>._)).Returns(createEnroll);
            A.CallTo(() => _eventService.GetNumberOfEnrolls(A<int>._)).Returns(11);

            // Act
            var result = await _eventsController.EventEnrollment(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_MaxNumberOfEnrollments");
        }

        [Fact]
        public async Task EventsController_EventEnrollment_ReturnsDateHigher()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, MaxEventEnrolls = 10, EnrollLimitDate = DateTime.UtcNow };
            var role = new UsersRoleClub { ClubId = 1 };
            var createEnroll = new EventEnroll { EventId = 1, UserId = "1" };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(Task.FromResult<EventEnroll>(null));
            A.CallTo(() => _eventService.CreateEventEnroll(A<EventEnroll>._)).Returns(createEnroll);
            A.CallTo(() => _eventService.GetNumberOfEnrolls(A<int>._)).Returns(1);

            // Act
            var result = await _eventsController.EventEnrollment(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_LimitDateExceed");
        }

        [Fact]
        public async Task EventsController_CancelEventEnroll_ReturnsSuccess()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, Fee = 0, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } } };
            var enroll = new EventEnroll { EventId = 1, UserId = "1", EnrollStatus = EnrollPaymentStatus.Valid };
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(enroll);

            // Act
            var result = await _eventsController.CancelEventEnroll(1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Details");
        }

        [Fact]
        public async Task EventsController_CancelEventEnroll_ReturnsIdNull()
        {
            // Arrange

            // Act
            var result = await _eventsController.CancelEventEnroll(null);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_CancelEventEnroll_ReturnsEventNull()
        {
            // Arrange
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));

            // Act
            var result = await _eventsController.CancelEventEnroll(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }


        [Fact]
        public async Task EventsController_CancelEventEnroll_ReturnsEnrollStatus()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, Fee = 10, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } } };
            var enroll = new EventEnroll { EventId = 1, UserId = "1", EnrollStatus = EnrollPaymentStatus.Valid };
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(enroll);

            // Act
            var result = await _eventsController.CancelEventEnroll(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_CancelEventEnroll_ReturnsEnrollRoRemoveNull()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } } };
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(Task.FromResult<EventEnroll>(null));

            // Act
            var result = await _eventsController.CancelEventEnroll(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_PathInfoMapBox_ReturnsSuccess()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, Route = "Rota" };
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _eventService.GetEnroll(A<int>._, A<string>._)).Returns(A.Fake<EventEnroll>());

            // Act
            var result = await _eventsController.PathInfoMapBox(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EventsController_PathInfoMapBox_ReturnsEventNull()
        {
            // Arrange
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));

            // Act
            var result = await _eventsController.PathInfoMapBox(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_PathInfoMapBox_ReturnsRouteNull()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, Route = null };
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);

            // Act
            var result = await _eventsController.PathInfoMapBox(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_UpdateEventLocation_ReturnsSuccess()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, Route = null };
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.UpdateEventLocation(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EventsController_UpdateEventLocation_ReturnsEventNotFound()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.UpdateEventLocation(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_UpdateEventLocation_ReturnsClubIdDiff()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 2 };
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, Route = null };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.UpdateEventLocation(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_UpdateLocation_ReturnsSuccess()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, Route = null };
            var role = new UsersRoleClub { ClubId = 1 };
            var address = new Address
            {
                CoordinateY = 38.5225957,
                CoordinateX = -8.8390330,
                AddressString = "R Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal"
            };

            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.CreateEventAddress(A<Address>._)).Returns(A.Fake<Address>());

            // Act
            var result = await _eventsController.UpdateLocation(1, address);

            // Assert
            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task EventsController_UpdateLocation_ReturnsRoleNull()
        {
            // Arrange
            var address = new Address
            {
                CoordinateY = 38.5225957,
                CoordinateX = -8.8390330,
                AddressString = "R Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal"
            };

            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(Task.FromResult<UsersRoleClub>(null));

            // Act
            var result = await _eventsController.UpdateLocation(1, address);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_UpdateLocation_ReturnsEventNull()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 1 };
            var address = new Address
            {
                CoordinateY = 38.5225957,
                CoordinateX = -8.8390330,
                AddressString = "R Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal"
            };

            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));

            // Act
            var result = await _eventsController.UpdateLocation(1, address);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_UpdateLocation_ReturnsClubIdDiff()
        {
            // Arrange
            var e = new Event { Id = 2, IsPublic = true, ClubId = 2, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, Route = null };
            var role = new UsersRoleClub { ClubId = 1 };
            var address = new Address
            {
                CoordinateY = 38.5225957,
                CoordinateX = -8.8390330,
                AddressString = "R Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal"
            };

            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.UpdateLocation(1, address);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_UpdateLocation_ReturnsSuccessUpdate()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, LocationId = 1 };
            var role = new UsersRoleClub { ClubId = 1 };
            var address = new Address
            {
                CoordinateY = 38.5225957,
                CoordinateX = -8.8390330,
                AddressString = "R Estefanilha 1, 2910-846 Setúbal, Setúbal, Portugal"
            };

            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.UpdateLocation(1, address);

            // Assert
            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task EventsController_GetAllEvents_ReturnsSuccess()
        {
            // Arrange
            A.CallTo(() => _eventService.GetEvents(A<string>._)).Returns(A.Fake<IEnumerable<Event>>());

            // Act
            var result = await _eventsController.GetAllEvents();

            // Assert
            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task EventsController_Results_ReturnsSuccess()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Time };
            var role = new UsersRoleClub { ClubId = 1 };

            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetResults(A<int>._)).Returns(A.Fake<ICollection<EventResult>>());

            // Act
            var result = await _eventsController.Results(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EventsController_Results_ReturnsSuccessScore()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Score };
            var role = new UsersRoleClub { ClubId = 1 };

            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetResults(A<int>._)).Returns(A.Fake<ICollection<EventResult>>());

            // Act
            var result = await _eventsController.Results(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EventsController_Results_ReturnsSuccessPosition()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Position };
            var role = new UsersRoleClub { ClubId = 1 };

            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetResults(A<int>._)).Returns(A.Fake<ICollection<EventResult>>());

            // Act
            var result = await _eventsController.Results(1);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EventsController_AddResult_ReturnsSuccess()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Position };
            var role = new UsersRoleClub { ClubId = 1 };
            var enrolls = new List<EventEnroll>() { new EventEnroll { EnrollStatus = EnrollPaymentStatus.Valid } };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetResults(A<int>._)).Returns(A.Fake<ICollection<EventResult>>());
            A.CallTo(() => _eventService.GetEnrolls(A<int>._)).Returns(enrolls);


            // Act
            var result = await _eventsController.AddResult(1);

            // Assert
            result.Should().BeOfType<PartialViewResult>().Which.ViewName.Should().Be("_PartialAddResult");
        }

        [Fact]
        public async Task EventsController_AddResult_ReturnsEventNull()
        {
            // Arrange
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));

            // Act
            var result = await _eventsController.AddResult(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_AddResult_ReturnsClubIdDiff()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, ClubId = 2, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Position };
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.AddResult(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");

        }

        [Fact]
        public async Task EventsController_AddResult_ReturnsCountZero()
        {
            // Arrange
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Time };
            var role = new UsersRoleClub { ClubId = 1 };
            var enrolls = new List<EventEnroll>() { new EventEnroll { UserId = "1", EnrollStatus = EnrollPaymentStatus.Valid } };
            var results = new List<EventResult>() { new EventResult { UserId = "1", } };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.GetResults(A<int>._)).Returns(results);
            A.CallTo(() => _eventService.GetEnrolls(A<int>._)).Returns(enrolls);

            // Act
            var result = await _eventsController.AddResult(1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NoUsersToResult");
        }

        [Fact]
        public async Task EventsController_AddResult_Post_Returns()
        {
            // Arrange
            var res = new ResultModel { Result = "2.0", UserId = "1" };
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Time };
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.CreateResult(A<EventResult>._)).Returns(A.Fake<EventResult>());

            // Act
            var result = await _eventsController.AddResult(1, res);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Results");
        }

        [Fact]
        public async Task EventsController_AddResult_Post_ReturnsScore()
        {
            // Arrange
            var res = new ResultModel { Result = "2", UserId = "1" };
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Score };
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.CreateResult(A<EventResult>._)).Returns(A.Fake<EventResult>());

            // Act
            var result = await _eventsController.AddResult(1, res);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Results");
        }

        [Fact]
        public async Task EventsController_AddResult_Post_ReturnsPosition()
        {
            // Arrange
            var res = new ResultModel { Result = "2", UserId = "1" };
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Position };
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);
            A.CallTo(() => _eventService.CreateResult(A<EventResult>._)).Returns(A.Fake<EventResult>());

            // Act
            var result = await _eventsController.AddResult(1, res);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Results");
        }

        [Fact]
        public async Task EventsController_AddResult_Post_ReturnsEventNull()
        {
            // Arrange
            var res = new ResultModel { Result = "2.0", UserId = "1" };
            var role = new UsersRoleClub { ClubId = 1 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(Task.FromResult<Event>(null));
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.AddResult(1, res);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_AddResult_Post_ReturnsClubIdDiff()
        {
            // Arrange
            var res = new ResultModel { Result = "2.0", UserId = "1" };
            var e = new Event { Id = 1, IsPublic = true, ClubId = 1, EventTranslations = A.Fake<ICollection<EventTranslation>>(), UsersEnrolled = new List<EventEnroll> { new EventEnroll { UserId = "" } }, EventResultType = ResultType.Time };
            var role = new UsersRoleClub { ClubId = 2 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetEvent(A<int>._)).Returns(e);
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.AddResult(1, res);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_DeleteResult_ReturnsSuccess()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 2 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetResult(A<string>._, A<int>._)).Returns(A.Fake<EventResult>());
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.DeleteResult("", 1);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Results");
        }

        [Fact]
        public async Task EventsController_DeleteResult_ReturnsUserIdNull()
        {
            // Arrange

            // Act
            var result = await _eventsController.DeleteResult(null, 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }

        [Fact]
        public async Task EventsController_DeleteResult_ReturnsRoleNull()
        {
            // Arrange
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(Task.FromResult<UsersRoleClub>(null));
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.DeleteResult("", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_Unauthorized");
        }

        [Fact]
        public async Task EventsController_DeleteResult_ReturnsDeleteNull()
        {
            // Arrange
            var role = new UsersRoleClub { ClubId = 2 };
            A.CallTo(() => _userService.GetSelectedRole(A<string>._)).Returns(role);
            A.CallTo(() => _eventService.GetResult(A<string>._, A<int>._)).Returns(Task.FromResult<EventResult>(null));
            A.CallTo(() => _clubService.IsClubStaff(A<UsersRoleClub>._)).Returns(true);

            // Act
            var result = await _eventsController.DeleteResult("", 1);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("CustomError");
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be("Error_NotFound");
        }
    }
}
