using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.TeamService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCManagement.Tests.Services
{
    public class TeamServiceTests
    {
        private readonly TeamService _teamService;
        private readonly ApplicationDbContext _context;

        public TeamServiceTests()
        {
            _context = GetDbContext().Result;

            //SUT (system under test)
            _teamService = new TeamService(_context);
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SCManagementTeam")
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

                context.Club.Add(new Club
                {
                    Id = 2,
                    Name = $"Test Club 2",
                    CreationDate = DateTime.Now,
                    Modalities = new List<Modality>
                        {
                            context.Modality.FirstOrDefault(m => m.Id == 1)
                        },
                    UsersRoleClub = new List<UsersRoleClub>
                        {
                            new UsersRoleClub { UserId = "Test 1" , RoleId = 20 },
                            new UsersRoleClub { UserId = "Test 2" , RoleId = 20 },
                            new UsersRoleClub { UserId = "Test 3" , RoleId = 30 },
                        },
                    ClubTranslations = new List<ClubTranslations>
                        {
                            new ClubTranslations
                            {
                                ClubId = 2,
                                Value = "",
                                Language = "en-US",
                                Atribute = "TermsAndConditions",
                            },
                            new ClubTranslations
                            {
                                ClubId = 2,
                                Value = "",
                                Language = "en-US",
                                Atribute = "About",
                            }
                        }
                });

                await context.SaveChangesAsync();
            }
            
            if (!await context.Team.AnyAsync())
            {
                context.Team.AddRange(new Team
                {
                    Id = 1,
                    Name = "Team 1",
                    ClubId = 1,
                    ModalityId = 1,
                    CreationDate = DateTime.Now,
                    TrainerId = "Test 3",
                    Athletes = new List<User>
                    {
                        context.Users.FirstOrDefault(u => u.Id == "Test 2"),
                    }
                },
                new Team
                {
                    Id = 2,
                    Name = "Team 2",
                    ClubId = 1,
                    ModalityId = 5,
                    CreationDate = DateTime.Now,
                    TrainerId = "Test 3",
                    Athletes = new List<User>
                    {
                        context.Users.FirstOrDefault(u => u.Id == "Test 6"),
                    }
                },
                new Team
                {
                    Id = 3,
                    Name = "Team 3",
                    ClubId = 1,
                    ModalityId = 2,
                    CreationDate = DateTime.Now,
                    TrainerId = "Test 3",
                    Athletes = new List<User>
                    {
                        context.Users.FirstOrDefault(u => u.Id == "Test 5"),
                    }
                },
                new Team
                {
                    Id = 4,
                    Name = "Team 4",
                    ClubId = 2,
                    ModalityId = 2,
                    CreationDate = DateTime.Now,
                    TrainerId = "Test 3",
                    Athletes = new List<User>
                    {
                        context.Users.FirstOrDefault(u => u.Id == "Test 1"),
                        context.Users.FirstOrDefault(u => u.Id == "Test 2")
                    }
                },
                new Team
                {
                    Id = 5,
                    Name = "Team 5",
                    ClubId = 2,
                    ModalityId = 2,
                    CreationDate = DateTime.Now,
                    TrainerId = "Test 3",
                    Athletes = new List<User>
                    {
                        context.Users.FirstOrDefault(u => u.Id == "Test 2"),
                    }
                });
                
                await context.SaveChangesAsync();
            }

            return context;
        }

        [Fact]
        public async Task TeamService_GetTeam_ReturnsTeam()
        {
            // Arrange

            // Act
            var result = await _teamService.GetTeam(1);

            // Assert
            result.Should().BeOfType<Team>().Which.Name.Should().Be("Team 1");
        }

        [Fact]
        public async Task TeamService_GetTeam_ReturnsNull()
        {
            // Arrange

            // Act
            var result = await _teamService.GetTeam(1000);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task TeamService_GetTeams_ReturnsTeams()
        {
            // Arrange

            // Act
            var result = await _teamService.GetTeams(1);

            // Assert
            result.Should().BeOfType<List<Team>>().Which.Count.Should().Be(3);
        }

        [Fact]
        public async Task TeamService_GetTeams_ReturnsEmptyList()
        {
            // Arrange

            // Act
            var result = await _teamService.GetTeams(10000);

            // Assert
            result.Should().BeOfType<List<Team>>().Which.Count.Should().Be(0);
        }

        [Fact]
        public async Task TeamService_CreateTeam_ReturnsSuccess()
        {
            // Arrange
            var team = new Team
            {
                Name = "Team 4",
                ClubId = 1,
                ModalityId = 2,
                CreationDate = DateTime.Now,
                TrainerId = "Test 3",
                Athletes = new List<User>
                    {
                        _context.Users.FirstOrDefault(u => u.Id == "Test 2"),
                    }
            };

            // Act
            var result = await _teamService.CreateTeam(team);

            // Assert
            result.Should().BeOfType<Team>();
            _context.Team.FirstOrDefault(t => t.Name == "Team 2").Should().NotBeNull();
            await _teamService.DeleteTeam(team);
        }

        [Fact]
        public async Task TeamService_UpdateTeam_ReturnsSuccess()
        {
            // Arrange
            var team = new Team
            {
                Name = "Team X",
                ClubId = 1,
                ModalityId = 2,
                CreationDate = DateTime.Now,
                TrainerId = "Test 3",
                Athletes = new List<User>
                    {
                        _context.Users.FirstOrDefault(u => u.Id == "Test 2"),
                    }
            };

            // Act
            await _teamService.CreateTeam(team);
            team.Name = "Team 21111";
            var result = await _teamService.UpdateTeam(team);

            // Assert
            result.Should().BeOfType<Team>();
            _context.Team.FirstOrDefault(t => t.Name == "Team X").Should().BeNull();
            await _teamService.DeleteTeam(team);
        }

        [Fact]
        public async Task TeamService_UpdateTeamAthletes_ReturnsSuccess()
        {
            // Arrange
            var athletes = new List<string>
            {
                "Test 2",
                "Test 5",
                "Test 6",
            };

            // Act
            await _teamService.UpdateTeamAthletes(1, athletes);

            // Assert
            _context.Team.FirstOrDefault(t => t.Id == 1).Athletes.Count.Should().Be(3);
        }

        [Fact]
        public async Task TeamService_UpdateTeamAthletes_ReturnsNull()
        {
            // Arrange

            // Act
            await _teamService.UpdateTeamAthletes(2, new List<string>());

            // Assert
            _context.Team.FirstOrDefault(t => t.Id == 2).Athletes.Count.Should().Be(1);
        }

        [Fact]
        public async Task TeamService_DeleteTeam_ReturnsSuccess()
        {
            // Arrange
            var team = new Team
            {
                Name = "Team 1223",
                ClubId = 1,
                ModalityId = 2,
                CreationDate = DateTime.Now,
                TrainerId = "Test 3",
                Athletes = new List<User>
                    {
                        _context.Users.FirstOrDefault(u => u.Id == "Test 2"),
                    }
            };

            // Act
            await _teamService.CreateTeam(team);
            await _teamService.DeleteTeam(team);

            // Assert
            _context.Team.FirstOrDefault(t => t.Name == "Team 1223").Should().BeNull();
        }

        [Fact]
        public async Task TeamService_GetTeamsByAthlete_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _teamService.GetTeamsByAthlete("Test 2");

            // Assert
            result.Should().BeOfType<List<Team>>().Which.Count.Should().Be(3);
        }

        [Fact]
        public async Task TeamService_GetTeamsByAthleteWithClub_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _teamService.GetTeamsByAthlete("Test 6", 1);

            // Assert
            result.Should().BeOfType<List<Team>>().Which.Count.Should().Be(1);
        }

        [Fact]
        public async Task TeamService_RemoveAthlete_ReturnsSuccess()
        {
            // Arrange
            var team = _context.Team.Include(u => u.Athletes).FirstOrDefault(t => t.Id == 3);
            var athlete = _context.Users.FirstOrDefault(u => u.Id == "Test 5");

            // Act
            await _teamService.RemoveAthlete(team, athlete);

            // Assert
            _context.Team.FirstOrDefault(t => t.Id == 3).Athletes.Count.Should().Be(0);
        }
    }
}
