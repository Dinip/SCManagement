using FakeItEasy;
using FakeItEasy.Creation;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.PlansService;
using SCManagement.Services.PlansService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCManagement.Tests.Services
{
    public class PlanServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IPlanService _planService;

        public PlanServiceTests()
        {
            _context = GetDbContext().Result;

            _planService = new PlanService(_context);
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SCManagementPlans")
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
                });


                await context.SaveChangesAsync();
            }

            if (!await context.TrainingPlans.AnyAsync())
            {
                context.TrainingPlans.AddRange(new TrainingPlan
                {
                    Id = 1,
                    Name = "Treino 1",
                    Description = "Treino de teste",
                    AthleteId = "Test 6",
                    TrainerId = "Test 3",
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
                }, 
                new TrainingPlan
                {
                    Id = 2,
                    Name = "Treino 2",
                    Description = "Treino de teste",
                    TrainerId = "Test 3",
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
                });

                await context.SaveChangesAsync();
            }

            if (!await context.MealPlans.AnyAsync())
            {
                context.MealPlans.AddRange(new MealPlan
                {
                    Id = 1,
                    Name = "Treino 1",
                    Description = "Treino de teste",
                    TrainerId = "Test 3",
                    AthleteId = "Test 6",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    IsTemplate = false,
                    MealPlanSessions = new List<MealPlanSession>
                    {
                        new MealPlanSession
                        {
                            MealName= "Exercicio 1",
                            MealDescription = "Aerobico",
                            Time = DateTime.Now.TimeOfDay
                        }
                    }
                },
                new MealPlan
                {
                    Id = 2,
                    Name = "Treino 2",
                    Description = "Treino de teste",
                    TrainerId = "Test 3",
                    IsTemplate = true,
                    MealPlanSessions = new List<MealPlanSession>
                    {
                        new MealPlanSession
                        {
                            MealName= "Exercicio 1",
                            MealDescription = "Aerobico",
                            Time = DateTime.Now.TimeOfDay
                        }
                    }
                });

                await context.SaveChangesAsync();
            }
            
            if (!await context.Goals.AnyAsync())
            {
                context.Goals.AddRange(new Goal
                {
                    Id = 1,
                    Name = "Goal 1",
                    Description = "Description 1",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(10),
                    TrainerId = "Test 3",
                    AthleteId = "Test 6",
                    isCompleted = false
                },
                new Goal
                {
                    Id = 2,
                    Name = "Goal 2",
                    Description = "Description 2",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(10),
                    TrainerId = "Test 3",
                    AthleteId = "Test 6",
                    isCompleted = true
                });
            }

            return context;
        }

        
        [Fact]
        public async Task PlanService_CreateTrainingPlan_ReturnsPlans()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Name = "Treino 1",
                Description = "Treino de teste",
                AthleteId = "Test 6",
                TrainerId = "Test 3",
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
            var plans = new List<TrainingPlan> { trainingPlan };

            // Act
            var result = await _planService.CreateTrainingPlan(plans);

            // Assert
            result.Should().BeOfType<List<TrainingPlan>>();
        }

        [Fact]
        public async Task PlanService_CreateMealPlan_ReturnsPlans()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Name = "Treino 1",
                Description = "Treino de teste",
                TrainerId = "Test 3",
                AthleteId = "Test 6",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };

            var plans = new List<MealPlan> { mealPlan };

            // Act
            var result = await _planService.CreateMealPlan(plans);

            // Assert
            result.Should().BeOfType<List<MealPlan>>();
        }

        [Fact]
        public async Task PlanService_GetTrainingPlans_ReturnsTrainerPlans()
        {
            // Arrange

            // Act
            var result = await _planService.GetTrainingPlans("Test 3");

            // Assert
            result.Should().BeOfType<List<TrainingPlan>>();
        }

        [Fact]
        public async Task PlanService_GetTrainingPlans_ReturnsAthletePlans()
        {
            // Arrange

            // Act
            var result = await _planService.GetTrainingPlans("Test 3", "Test 6");

            // Assert
            result.Should().BeOfType<List<TrainingPlan>>();
        }

        [Fact]
        public async Task PlanService_GetMyTrainingPlans_ReturnsAthletePlans()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyTrainingPlans("Test 6",0);

            // Assert
            result.Should().BeOfType<List<TrainingPlan>>();
        }

        [Fact]
        public async Task PlanService_GetMyTrainingPlans_ReturnsAthletePlansFuture()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyTrainingPlans("Test 6", 1);

            // Assert
            result.Should().BeOfType<List<TrainingPlan>>();
        }

        [Fact]
        public async Task PlanService_GetMyTrainingPlans_ReturnsAthletePlansFinished()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyTrainingPlans("Test 6", 2);

            // Assert
            result.Should().BeOfType<List<TrainingPlan>>();
        }

        [Fact]
        public async Task PlanService_GetMealPlans_ReturnsTrainerPlans()
        {
            // Arrange

            // Act
            var result = await _planService.GetMealPlans("Test 3");

            // Assert
            result.Should().BeOfType<List<MealPlan>>();
        }

        [Fact]
        public async Task PlanService_GetMealPlans_ReturnsAthletePlans()
        {
            // Arrange

            // Act
            var result = await _planService.GetMealPlans("Test 3", "Test 6");

            // Assert
            result.Should().BeOfType<List<MealPlan>>();
        }

        [Fact]
        public async Task PlanService_GetMyMealPlans_ReturnsAthletePlans()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyMealPlans("Test 6", 0);

            // Assert
            result.Should().BeOfType<List<MealPlan>>();
        }

        [Fact]
        public async Task PlanService_GetMyMealPlans_ReturnsAthletePlansFuture()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyMealPlans("Test 6", 1);

            // Assert
            result.Should().BeOfType<List<MealPlan>>();
        }

        [Fact]
        public async Task PlanService_GetMyMealPlans_ReturnsAthletePlansFinished()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyMealPlans("Test 6", 2);

            // Assert
            result.Should().BeOfType<List<MealPlan>>();
        }

        [Fact]
        public async Task PlanService_GetTrainingPlan_ReturnsTrainingPlan()
        {
            // Arrange

            // Act
            var result = await _planService.GetTrainingPlan(1);

            // Assert
            result.Should().BeOfType<TrainingPlan>();
        }

        [Fact]
        public async Task PlanService_GetMealPlan_ReturnsMealPlan()
        {
            // Arrange

            // Act
            var result = await _planService.GetMealPlan(1);

            // Assert
            result.Should().BeOfType<MealPlan>();
        }

        [Fact]
        public async Task PlanService_UpdateTrainingPlan_ReturnsTrainingPlan()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Name = "Treino 1",
                Description = "Treino de teste",
                AthleteId = "Test 6",
                TrainerId = "Test 3",
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
            var list = new List<TrainingPlan> { trainingPlan };

            // Act
            var listResult = await _planService.CreateTrainingPlan(list);
            listResult.First().Name = "Treino ToUpdate";
            var result = await _planService.UpdateTrainingPlan(listResult.First());

            // Assert
            result.Should().BeOfType<TrainingPlan>().Which.Name.Should().Be("Treino ToUpdate");
        }

        [Fact]
        public async Task PlanService_UpdateMealPlan_ReturnsMealPlan()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Name = "Treino 1",
                Description = "Treino de teste",
                TrainerId = "Test 3",
                AthleteId = "Test 6",
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };

            var list = new List<MealPlan> { mealPlan };

            // Act
            var listResult = await _planService.CreateMealPlan(list);
            listResult.First().Name = "Treino ToUpdate";
            var result = await _planService.UpdateMealPlan(listResult.First());

            // Assert
            result.Should().BeOfType<MealPlan>().Which.Name.Should().Be("Treino ToUpdate");
        }
        [Fact]
        public async Task PlanService_DeleteTrainingPlan_ReturnsTrainingPlan()
        {
            // Arrange
            var trainingPlan = new TrainingPlan
            {
                Name = "Treino 1",
                Description = "Treino de teste",
                AthleteId = "Test 6",
                TrainerId = "Test 3",
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
            var count = _context.TrainingPlans.Count();

            var list = new List<TrainingPlan>() { trainingPlan};

            // Act
            await _planService.CreateTrainingPlan(list);
            await _planService.DeleteTrainingPlan(trainingPlan);

            // Assert
            _context.TrainingPlans.Count().Should().Be(count);
        }

        [Fact]
        public async Task PlanService_DeleteMealPlan_ReturnsMealPlan()
        {
            // Arrange
            var mealPlan = new MealPlan
            {
                Name = "Treino 1",
                Description = "Treino de teste",
                TrainerId = "Test 3",
                AthleteId = "Test 6",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                IsTemplate = false,
                MealPlanSessions = new List<MealPlanSession>
                {
                    new MealPlanSession
                    {
                        MealName= "Exercicio 1",
                        MealDescription = "Aerobico",
                        Time = DateTime.Now.TimeOfDay
                    }
                }
            };
            var count = _context.MealPlans.Count();

            var list = new List<MealPlan>() { mealPlan };

            // Act
            await _planService.CreateMealPlan(list);
            await _planService.DeleteMealPlan(mealPlan);

            // Assert
            _context.MealPlans.Count().Should().Be(count);
        }

        [Fact]
        public async Task PlanService_GetTemplateTrainingPlan_ReturnsTemplates()
        {
            // Arrange

            // Act
            var result = await _planService.GetTemplateTrainingPlan(2);

            // Assert
            result.Should().BeOfType<TrainingPlan>();
        }

        [Fact]
        public async Task PlanService_GetTemplateMealPlan_ReturnsTemplates()
        {
            // Arrange

            // Act
            var result = await _planService.GetTemplateMealPlan(2);

            // Assert
            result.Should().BeOfType<MealPlan>();
        }

        [Fact]
        public async Task PlanService_GetTemplateTrainingPlans_ReturnsTemplates()
        {
            // Arrange

            // Act
            var result = await _planService.GetTemplateTrainingPlans("Test 3");

            // Assert
            result.Should().BeOfType<List<TrainingPlan>>();
        }

        [Fact]
        public async Task PlanService_GetTemplateMealPlans_ReturnsTemplates()
        {
            // Arrange

            // Act
            var result = await _planService.GetTemplateMealPlans("Test 3");

            // Assert
            result.Should().BeOfType<List<MealPlan>>();
        }

        [Fact]
        public async Task PlanService_GetGoals_ReturnsGoals()
        {
            // Arrange

            // Act
            var result = await _planService.GetGoals("Test 3");

            // Assert
            result.Should().BeOfType<List<Goal>>();
        }

        [Fact]
        public async Task PlanService_GetGoals_ReturnsAthleteGoals()
        {
            // Arrange

            // Act
            var result = await _planService.GetGoals("Test 3", "Test 6");

            // Assert
            result.Should().BeOfType<List<Goal>>();
        }

        [Fact]
        public async Task PlanService_GetGoal_ReturnsGoal()
        {
            // Arrange

            // Act
            var result = await _planService.GetGoal(1);

            // Assert
            result.Should().BeOfType<Goal>();
        }

        [Fact]
        public async Task PlanService_CreateGoal_ReturnsGoal()
        {
            // Arrange
            var goal = new Goal
            {
                Name = "Goal 1",
                Description = "Description 1",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                TrainerId = "Test 3",
                AthleteId = "Test 6",
                isCompleted = false
            };

            // Act
            var result = await _planService.CreateGoal(goal);

            // Assert
            result.Should().BeOfType<Goal>();
        }

        [Fact]
        public async Task PlanService_UpdateGoal_ReturnsUpdateGoal()
        {
            // Arrange
            var goal = new Goal
            {
                Id = 1,
                Name = "Goal 1",
                Description = "Description 1",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                TrainerId = "Test 3",
                AthleteId = "Test 6",
                isCompleted = true
            };

            // Act
            var result = await _planService.UpdateGoal(goal);

            // Assert
            result.Should().BeOfType<Goal>();
        }

        [Fact]
        public async Task PlanService_DeleteGoal_ReturnsDeleteGoal()
        {
            // Arrange
            var goal = new Goal
            {
                Id = 10,
                Name = "SDFGSDFG",
                Description = "SDFGSDFG",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                TrainerId = "Test 3",
                AthleteId = "Test 6",
                isCompleted = true
            };
            await _planService.CreateGoal(goal);
            var count = _context.Goals.Count();

            // Act
            await _planService.DeleteGoal(goal);

            // Assert
            _context.Goals.Count().Should().Be(count-1);
        }

        [Fact]
        public async Task PlanService_GetMyGoals_ReturnsAthleteGoals()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyGoals("Test 6",0);

            // Assert
            result.Should().BeOfType<List<Goal>>();
        }
        
        [Fact]
        public async Task PlanService_GetMyGoals_ReturnsAthleteGoalsNotFinished()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyGoals("Test 6", 1);

            // Assert
            result.Should().BeOfType<List<Goal>>();
        }
        
        [Fact]
        public async Task PlanService_GetMyGoals_ReturnsAthleteGoalsFinished()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyGoals("Test 6", 2);

            // Assert
            result.Should().BeOfType<List<Goal>>();
        }
        
        [Fact]
        public async Task PlanService_GetMyGoals_ReturnsAthleteGoalsAll()
        {
            // Arrange

            // Act
            var result = await _planService.GetMyGoals("Test 6", 3);

            // Assert
            result.Should().BeOfType<List<Goal>>();
        }

        [Fact]
        public async Task PlanService_GetModalities_ReturnsAthleteModalities()
        {
            // Arrange

            // Act
            var result = await _planService.GetModalities("Test 6", "Test 3");

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task PlanService_GetModalities_ReturnsTrainerModalities()
        {
            // Arrange

            // Act
            var result = await _planService.GetModalities("Test 3");

            // Assert
            result.Should().NotBeEmpty();
        }
    }
}
