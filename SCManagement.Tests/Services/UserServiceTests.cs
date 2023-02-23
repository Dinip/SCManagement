using Microsoft.AspNetCore.Identity;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using Microsoft.Extensions.Configuration;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using FakeItEasy;

namespace SCManagement.Tests.Services
{
    public class UserServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _context = GetDbContext().Result;

            //SUT (system under test)
            _userService = new UserService(_context, A.Fake<SignInManager<User>>());
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SCManagementClubUserService")
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

            return context;
        }

        [Fact]
        public async Task UserService_GetTeam_ReturnsSuccess()
        {
            // Arrange
            var user = _context.Users.Find("Test 3");
            user.EmailConfirmed = true;

            // Act
            await _userService.UpdateUser(user);

            // Assert
            _context.Users.Find("Test 3").EmailConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task UserService_GetUser_ReturnsSuccess()
        {
            // Arrange
            
            // Act
            var result = await _userService.GetUser("Test 1");

            // Assert
            result.Should().BeOfType<User>();
        }

        [Fact]
        public async Task UserService_GetUser_ReturnsNull()
        {
            // Arrange

            // Act
            var result = await _userService.GetUser("abcd");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UserService_GetUserWithRoles_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _userService.GetUserWithRoles("Test 1");

            // Assert
            result.Should().BeOfType<User>();
        }

        [Fact]
        public async Task UserService_GetUserWithRoles_ReturnsNull()
        {
            // Arrange

            // Act
            var result = await _userService.GetUserWithRoles("asdsdg");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UserService_UpdateSelectedRole_ReturnsSuccess()
        {
            // Arrange

            // Act
            await _userService.UpdateSelectedRole("Test 1",1);

            // Assert
            _context.UsersRoleClub.Where(u => u.UserId == "Test 1").FirstOrDefault().Selected.Should().Be(true);
        }

        [Fact]
        public async Task UserService_GetSelectedRole_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _userService.GetSelectedRole("Test 1");

            // Assert
            result.Should().BeOfType<UsersRoleClub>();
        }

        [Fact]
        public async Task UserService_IsAtleteInAnyClub_ReturnsFalse()
        {
            // Arrange

            // Act
            var result = await _userService.IsAtleteInAnyClub("Test 1");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UserService_IsAtleteInAnyClub_ReturnsTrue()
        {
            // Arrange

            // Act
            var result = await _userService.IsAtleteInAnyClub("Test 6");

            // Assert
            result.Should().BeTrue();
        }

    }
}
