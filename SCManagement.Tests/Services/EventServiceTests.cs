using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.EventService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCManagement.Tests.Services
{
    public class EventServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _context = GetDbContext().Result;

            //SUT (system under test)
            _eventService = new EventService(_context);
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SCManagementEvent")
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            if (!await context.Users.AnyAsync())
            {
                context.Users.Add(new User { Id = "Test 1", FirstName = "Tester", LastName = "1", Email = "a@gmail.com", UserName = "Tester 1" });
                context.Users.Add(new User { Id = "Test 2", FirstName = "Tester", LastName = "2", Email = "b@gmail.com", UserName = "Tester 2" });
                context.Users.Add(new User { Id = "Test 3", FirstName = "Tester", LastName = "3", Email = "c@gmail.com", UserName = "Tester 3" });
                context.Users.Add(new User { Id = "Test 4", FirstName = "Tester", LastName = "4", Email = "d@gmail.com", UserName = "Tester 4" });
                context.Users.Add(new User { Id = "Test 5", FirstName = "Tester", LastName = "5", Email = "e@gmail.com", UserName = "Tester 5" });
                context.Users.Add(new User { Id = "Test 6", FirstName = "Tester", LastName = "6", Email = "f@gmail.com", UserName = "Tester 6" });
                await context.SaveChangesAsync();
            }

            if (!await context.Club.AnyAsync())
            {
                context.Club.Add(new Club
                {
                    Id = 1,
                    Name = $"Test Club 1",
                    CreationDate = DateTime.Now,
                    Modalities = new List<Modality>
                        {
                            context.Modality.FirstOrDefault(m => m.Id == 1)
                        },
                    UsersRoleClub = new List<UsersRoleClub>
                        {
                            new UsersRoleClub { UserId = "Test 1" , RoleId = 50 },
                            new UsersRoleClub { UserId = "Test 2" , RoleId = 20 },
                            new UsersRoleClub { UserId = "Test 3" , RoleId = 30 },
                            new UsersRoleClub { UserId = "Test 4" , RoleId = 40 },
                            new UsersRoleClub { UserId = "Test 5" , RoleId = 20 },
                            new UsersRoleClub { UserId = "Test 6" , RoleId = 20 },
                        },
                    ClubTranslations = new List<ClubTranslations>
                        {
                            new ClubTranslations
                            {
                                ClubId = 1,
                                Value = "",
                                Language = "en-US",
                                Atribute = "TermsAndConditions",
                            },
                            new ClubTranslations
                            {
                                ClubId = 1,
                                Value = "",
                                Language = "en-US",
                                Atribute = "About",
                            }
                        }
                });

                await context.SaveChangesAsync();
            }

            if (!await context.Event.AnyAsync())
            {
                context.Event.Add(new Event
                {
                    Id = 1,
                    ClubId = 1,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(5),
                    IsPublic = true,
                    
                });

                await context.SaveChangesAsync();
            }


            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task EventService_CreateEvent_ReturnsSuccess()
        {
            // Arrange
            Event e = new Event()
            {
                Id = 2,
                ClubId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                IsPublic = true,
                EventTranslations = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 4,
                        Value = "Test",
                        Language = "en-US",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 4,
                        Value = "Test desc",
                        Language = "en-US",
                        Atribute = "Description",
                    }
                }
            };

            // Act
            var result = await _eventService.CreateEvent(e);

            // Assert
            result.Should().BeOfType<Event>().And.NotBeNull();
        }

        [Fact]
        public async Task EventService_DeleteEvent_ReturnsSuccess()
        {
            // Arrange
            Event e = new Event()
            {
                Id = 3,
                ClubId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                IsPublic = true,
                EventTranslations = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 4,
                        Value = "Test",
                        Language = "en-US",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 4,
                        Value = "Test desc",
                        Language = "en-US",
                        Atribute = "Description",
                    }
                }
            };

            // Act
            await _eventService.CreateEvent(e);
            await _eventService.DeleteEvent(e);

            // Assert
            _context.Event.Should().NotContain(e);
        }

        [Fact]
        public async Task EventService_GetEvent_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _eventService.GetEvent(1);

            // Assert
            result.Should().BeOfType<Event>().And.NotBeNull();
        }

        [Fact]
        public async Task EventService_UpdateEvent_ReturnsSuccess()
        {
            // Arrange
            Event e = new Event()
            {
                Id = 4,
                ClubId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                IsPublic = true,
                EventTranslations = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 4,
                        Value = "Test",
                        Language = "en-US",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 4,
                        Value = "Test desc",
                        Language = "en-US",
                        Atribute = "Description",
                    }
                }
            };

            // Act
            await _eventService.CreateEvent(e);
            
            var result = await _eventService.UpdateEvent(e);

            // Assert
            result.Should().BeOfType<Event>().And.NotBeNull();
        }

        [Fact]
        public async Task EventService_CreateEventEnroll_ReturnsSuccess()
        {
            // Arrange
            EventEnroll e = new EventEnroll()
            {
                Id = 1,
                EventId = 1,
                UserId = "Test 1",
                EnrollDate = DateTime.Now,
            };

            // Act
            var result = await _eventService.CreateEventEnroll(e);

            // Assert
            result.Should().BeOfType<EventEnroll>().And.NotBeNull();
        }

        [Fact]
        public async Task EventService_GetEnroll_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _eventService.GetEnroll(1, "Test 1");

            // Assert
            result.Should().BeOfType<EventEnroll>().And.NotBeNull();
        }

        [Fact]
        public async Task EventService_CancelEventEnroll_ReturnsSuccess()
        {
            // Arrange
            EventEnroll e = new EventEnroll()
            {
                Id = 2,
                EventId = 1,
                UserId = "Test 2",
                EnrollDate = DateTime.Now,
            };

            // Act
            await _eventService.CreateEventEnroll(e);
            await _eventService.CancelEventEnroll(e);

            // Assert
            _context.EventEnroll.Should().NotContain(e);
        }

        [Fact]
        public async Task EventService_GetNumberOfEnrolls_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _eventService.GetNumberOfEnrolls(1);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task EventService_GetEnrolls_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _eventService.GetEnrolls(1);

            // Assert
            result.Should().BeOfType<List<EventEnroll>>();
        }

        [Fact]
        public async Task EventService_GetEvents_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _eventService.GetEvents("Test 1");

            // Assert
            result.Should().BeOfType<List<Event>>();
        }

        [Fact]
        public async Task EventService_GetEvents_ReturnsUserIdNull()
        {
            // Arrange
            
            // Act
            var result = await _eventService.GetEvents(null);

            // Assert
            result.Should().BeOfType<List<Event>>();
        }

        [Fact]
        public async Task EventService_CreateEventAddress_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _eventService.CreateEventAddress(new Address
            {
                Id = 1,
                City = "Test",
                Country = "Test",
                Street = "Test",
                ZipCode = "Test",
                District = "Test",
                CoordinateX = 2.0d,
                CoordinateY = 2.0d
            });

            // Assert
            result.Should().BeOfType<Address>();
        }

        [Fact]
        public async Task EventService_UpdateEventAddress_ReturnsSuccess()
        {
            // Arrange

            // Act
            await _eventService.CreateEventAddress(new Address
            {
                Id = 2,
                City = "Test",
                Country = "Test",
                Street = "Test",
                ZipCode = "Test",
                District = "Test",
                CoordinateX = 2.0d,
                CoordinateY = 2.0d
            });
            
            var result = await _eventService.UpdateEventAddress(2,new Address { Id = 1, City = "Test", Country = "Test", Street = "Test", ZipCode = "Test" });

            // Assert
            result.Should().BeOfType<Address>();
        }

        [Fact]
        public async Task EventService_RemoveEventAddress_ReturnsSuccess()
        {
            // Arrange
            Event e = new Event()
            {
                Id = 5,
                ClubId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                IsPublic = true,
                AddressByPath = "",
                EventTranslations = new List<EventTranslation>
                {
                    new EventTranslation
                    {
                        EventId = 4,
                        Value = "Test",
                        Language = "en-US",
                        Atribute = "Name",
                    },
                    new EventTranslation
                    {
                        EventId = 4,
                        Value = "Test desc",
                        Language = "en-US",
                        Atribute = "Description",
                    }
                }
            };
            
            // Act
            await _eventService.CreateEvent(e);
            await _eventService.RemoveEventAddress(e);

            // Assert
            _context.Event.Where(e => e.Id == 5).First().Location.Should().BeNull();
        }

        [Fact]
        public async Task EventService_CreateResult_ReturnsSuccess()
        {
            // Arrange
            EventResult e = new EventResult()
            {
                Id = 1,
                EventId = 1,
                UserId = "Test 1",
            };

            // Act
            var result = await _eventService.CreateResult(e);

            // Assert
            result.Should().BeOfType<EventResult>();
        }

        [Fact]
        public async Task EventService_GetResults_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _eventService.GetResults(1);

            // Assert
            result.Should().BeOfType<List<EventResult>>();
        }

        [Fact]
        public async Task EventService_GetResult_ReturnsSuccess()
        {
            // Arrange
            

            // Act
            var result = await _eventService.GetResult("Test 1", 1);

            // Assert
            result.Should().BeOfType<EventResult>();
        }

        [Fact]
        public async Task EventService_DeleteResult_ReturnsSuccess()
        {
            // Arrange
            EventResult e = new EventResult()
            {
                Id = 2,
                EventId = 1,
                UserId = "Test 2",
            };

            // Act
            await _eventService.CreateResult(e);
            await _eventService.DeleteResult(e);

            // Assert
            _context.EventResult.Should().NotContain(e);
        }
    }
}
