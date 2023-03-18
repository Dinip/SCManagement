using Microsoft.AspNetCore.Http;
using SCManagement.Data;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using SCManagement.Services.ClubService;
using SCManagement.Models;
using FluentAssertions;
using System.Data;
using SCManagement.Services.AzureStorageService.Models;
using System.Collections.Generic;
using SCManagement.Services.PaymentService.Models;
using FluentAssertions.Common;
using SCManagement.Services.PaymentService;

namespace SCManagement.Tests.Services
{
    public class ClubServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SharedResourceService _sharedResource;
        private readonly IAzureStorage _azureStorage;
        private readonly ClubService _clubService;
        private readonly IPaymentService _paymentService;

        public ClubServiceTests()
        {
            _context = GetDbContext().Result;
            _emailSender = A.Fake<IEmailSender>();
            _httpContext = A.Fake<IHttpContextAccessor>();
            _sharedResource = A.Fake<SharedResourceService>();
            _azureStorage = A.Fake<IAzureStorage>();
            _paymentService = A.Fake<IPaymentService>();

            //SUT (system under test)
            _clubService = new ClubService(_context, _emailSender, _httpContext, _sharedResource, _azureStorage, _paymentService);
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SCManagementClub")
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            if (!await context.Club.AnyAsync())
            {
                for (int i = 1; i <= 10; i++)
                {
                    context.Users.Add(new User { Id = $"Test {i}", FirstName = "Tester", LastName = i.ToString(), Email = "a@gmail.com", UserName = $"Tester {i}" });
                    context.Club.Add(new Club
                    {
                        Id = i,
                        Name = $"Test Club {i}",
                        CreationDate = DateTime.Now,
                        Modalities = new List<Modality> 
                        { 
                            context.Modality.FirstOrDefault(m => m.Id == i) 
                        },
                        UsersRoleClub = new List<UsersRoleClub> 
                        {
                            new UsersRoleClub { UserId = $"Test {i}" , RoleId = 50 , ClubId = i}
                        },
                        ClubTranslations = new List<ClubTranslations> 
                        {
                            new ClubTranslations 
                            {
                                ClubId = i,
                                Value = "",
                                Language = "en-US",
                                Atribute = "TermsAndConditions",
                            },
                            new ClubTranslations
                            {
                                ClubId = i,
                                Value = "",
                                Language = "en-US",
                                Atribute = "About",
                            }
                        },
                        Address = new Address
                        {
                            Id = i,
                            Street = "Test Street",
                            District = "Test District",
                            City = "Test City",
                            Country = "Test Country",
                            ZipCode = "Test ZipCode",
                            CoordinateX = 0,
                            CoordinateY = 0,
                        },
                        ClubPaymentSettings = new ClubPaymentSettings(),
                    });

                    context.Subscription.Add(new Subscription
                    {
                        Id = i,
                        Frequency = SubscriptionFrequency.Monthly,
                        ClubId = i,
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now.AddMonths(1),
                        Value = i,
                        Product = new Product { AthleteSlots = i - 1 ,Name = $"Pod {i}", ProductType = ProductType.ClubSubscription },
                        UserId = $"Test {i}"
                    });

                    if (i == 9)
                    {
                        context.Club.Where(c => c.Id == 1).First().UsersRoleClub.Add(new UsersRoleClub { UserId = "Test 2", RoleId = 40 , ClubId = 1 });
                        context.Club.Where(c => c.Id == 1).First().UsersRoleClub.Add(new UsersRoleClub { UserId = "Test 3", RoleId = 10 , ClubId = 1 });
                        context.Club.Where(c => c.Id == 1).First().UsersRoleClub.Add(new UsersRoleClub { UserId = "Test 4", RoleId = 10 , ClubId = 1 });
                        context.Club.Where(c => c.Id == 5).First().UsersRoleClub.Add(new UsersRoleClub { UserId = "Test 1", RoleId = 10 , ClubId = 5 });
                        context.Club.Where(c => c.Id == 5).First().UsersRoleClub.Add(new UsersRoleClub { UserId = "Test 2", RoleId = 20 , ClubId = 5 });
                        context.Club.Where(c => c.Id == 5).First().UsersRoleClub.Add(new UsersRoleClub { UserId = "Test 3", RoleId = 30 , ClubId = 5 });
                        context.Club.Where(c => c.Id == 5).First().UsersRoleClub.Add(new UsersRoleClub { UserId = "Test 4", RoleId = 40 , ClubId = 5 });
                    }

                    await context.SaveChangesAsync();
                }
            }

            
            

            if (!await context.CodeClub.AnyAsync())
            {
                context.CodeClub.AddRange(new CodeClub
                {
                    ClubId = 2,
                    Code = "123456",
                    CreatedByUserId = "Test 1",
                    RoleId = 10,
                    ExpireDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59),
                    Approved = true
                },
                new CodeClub
                {
                    ClubId = 2,
                    Code = "654321",
                    CreatedByUserId = "Test 1",
                    RoleId = 20,
                    ExpireDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59),
                    Approved = false
                }, 
                new CodeClub 
                { 
                    ClubId = 2,
                    Code = "123456789",
                    CreatedByUserId = "Test 1",
                    RoleId = 10,
                    ExpireDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59),
                    Approved = true,
                    UsedByUserId = "Test 2"
                },
                new CodeClub
                {
                    ClubId = 2,
                    Code = "1234560987",
                    CreatedByUserId = "Test 1",
                    RoleId = 10,
                    ExpireDate = DateTime.Now,
                    Approved = true,
                },
                new CodeClub
                {
                    ClubId = 2,
                    Code = "12345612431",
                    CreatedByUserId = "Test 1",
                    RoleId = 10,
                    ExpireDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59),
                    Approved = true,
                    UsedByUserId = "Test 2"
                },
                new CodeClub
                {
                    ClubId = 1,
                    Code = "1241234523562456",
                    CreatedByUserId = "Test 1",
                    RoleId = 20,
                    ExpireDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59),
                    Approved = true
                },
                 new CodeClub
                 {
                     ClubId = 10,
                     Code = "12345214563456456734567",
                     CreatedByUserId = "Test 1",
                     RoleId = 20,
                     ExpireDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59),
                     Approved = true
                 }
                );
            }

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task ClubService_CreateClub_ReturnsSuccess()
        {
            // Arrange
            Club club = new Club
            {
                Name = $"Test Club {11}",
                ModalitiesIds = new List<int>{1},
            };

            // Act
            var result = await _clubService.CreateClub(club, "Matias");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Club>();
        }

        [Fact]
        public async Task ClubService_GetClub_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClub(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Club>();
        }

        [Fact]
        public async Task ClubService_GetClub_ReturnsNull()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClub(1000);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ClubService_GetClubs_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClubs();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Club>>();
        }

        [Fact]
        public async Task ClubService_UpdateClub_ReturnsSuccess()
        {
            // Arrange
            Club club = new Club
            {
                Name = $"Test Update Club {1}",
                ModalitiesIds = new List<int> { 1 },
            };

            // Act
            var result = await _clubService.UpdateClub(club);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Club>();
        }

        [Fact]
        public async Task ClubService_GenerateCode_ReturnsNotApproved()
        {
            // Arrange
            CodeClub codeToBeCreated = new CodeClub
            {
                ClubId = 1,
                CreatedByUserId = "Tester 10",
                RoleId = 10,
                ExpireDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59),
                Approved = false
            };

            // Act
            var result = await _clubService.GenerateCode(codeToBeCreated);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CodeClub>();
        }

        [Fact]
        public async Task ClubService_GenerateCode_ReturnsSuccess()
        {
            // Arrange
            CodeClub codeToBeCreated = new CodeClub
            {
                ClubId = 1,
                CreatedByUserId = "Test 1",
                RoleId = 10,
                ExpireDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59),
                Approved = true
            };

            // Act
            var result = await _clubService.GenerateCode(codeToBeCreated);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CodeClub>();
        }

        [Fact]
        public async Task ClubService_GetUserRoleInClub_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetUserRoleInClub("Test 1", 1);

            // Assert
            result.Should().BeOfType<UsersRoleClub>().Which.RoleId.Should().Be(50);
        }

        [Fact]
        public async Task ClubService_GetUserRoleInClub_ReturnsDefault()
        {
            // Arrange

            // Act
            var resultClubIdNotExist = await _clubService.GetUserRoleInClub("Test 1", 1000);
            var resultUserIdNotExist = await _clubService.GetUserRoleInClub("Tester 1", 1);

            // Assert
            resultClubIdNotExist.Should().BeOfType<UsersRoleClub>().Which.RoleId.Should().Be(0);
            resultUserIdNotExist.Should().BeOfType<UsersRoleClub>().Which.RoleId.Should().Be(0);
        }

        [Fact]
        public async Task ClubService_GetRoles_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetRoles();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IEnumerable<RoleClub>>();
        }

        [Fact]
        public async Task ClubService_GetCodes_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetCodes(2);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IEnumerable<CodeClub>>();
        }

        [Fact]
        public async Task ClubService_GetCodeWithInfos_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetCodeWithInfos(6);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<CodeClub>();
        }

        [Fact]
        public void ClubService_UserAlreadyInAClub_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _clubService.UserAlreadyInAClub("Test 1", 1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_UserAlreadyInAClub_ReturnsClubNull()
        {
            // Arrange

            // Act
            var result = _clubService.UserAlreadyInAClub("Test 1", null);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_UserAlreadyInAClub_ReturnsFalse()
        {
            // Arrange

            // Act
            var result = _clubService.UserAlreadyInAClub("Test 134234", 1);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ClubService_UseCode_ReturnsSuccess()
        {
            // Arrange
            var code = _context.CodeClub.Where(c => c.Id == 7).First();

            // Act
            var result = await _clubService.UseCode("Test 134234", code);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Value.Should().Be("Success");
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Key.Should().Be(true);
        }

        [Fact]
        public async Task ClubService_UseCode_ReturnsNoAvailableSlots()
        {
            // Arrange
            var code = _context.CodeClub.Where(x => x.Id == 6).First();

            // Act
            var result = await _clubService.UseCode("Test 134234", code);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Value.Should().Be("Code_ClubFull");
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Key.Should().Be(false);
        }

        [Fact]
        public async Task ClubService_UseCode_ReturnsCodeNull()
        {
            // Arrange

            // Act
            var result = await _clubService.UseCode("Test 134234", null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Value.Should().Be("Code_NotFound");
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Key.Should().Be(false);
        }

        [Fact]
        public async Task ClubService_UseCode_ReturnsCodeNotExist()
        {
            // Arrange

            // Act
            var result = await _clubService.UseCode("Test 134234", A.Fake<CodeClub>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Value.Should().Be("Code_NotFound");
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Key.Should().Be(false);
        }

        [Fact]
        public async Task ClubService_UseCode_ReturnsCodeNotApproved()
        {
            // Arrange
            var code = _context.CodeClub.Where(c => c.Id == 2).First();

            // Act
            var result = await _clubService.UseCode("Test 134234", code);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Value.Should().Be("Code_NotApproved");
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Key.Should().Be(false);
        }

        [Fact]
        public async Task ClubService_UseCode_ReturnsCodeUsed()
        {
            // Arrange
            var code = _context.CodeClub.Where(c => c.Id == 3).First();

            // Act
            var result = await _clubService.UseCode("Test 134234", code);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Value.Should().Be("Code_AlreadyUsed");
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Key.Should().Be(false);
        }

        [Fact]
        public async Task ClubService_UseCode_ReturnsCodeExperid()
        {
            // Arrange
            var code = _context.CodeClub.Where(c => c.Id == 4).First();

            // Act
            var result = await _clubService.UseCode("Test 134234", code);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Value.Should().Be("Code_Expired");
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Key.Should().Be(false);
        }

        [Fact]
        public async Task ClubService_UseCode_ReturnsUserAlreadyInAClub()
        {
            // Arrange
            var code = _context.CodeClub.Where(c => c.Id == 6).First();

            // Act
            var result = await _clubService.UseCode("Test 2", code);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Value.Should().Be("Code_AlreadyPart");
            result.Should().BeOfType<KeyValuePair<bool, string>>().Which.Key.Should().Be(false);
        }

        [Fact]
        public void ClubService_IsClubAdmin_ReturnsSuccess()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 50 };

            // Act
            var result = _clubService.IsClubAdmin(userRoleClub);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_IsClubAdmin_ReturnsFail()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 50000 };

            // Act
            var result = _clubService.IsClubAdmin(userRoleClub);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_IsClubSecretary_ReturnsSuccess()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 40 };

            // Act
            var result = _clubService.IsClubSecretary(userRoleClub);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_IsClubSecretary_ReturnsFail()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 40000 };

            // Act
            var result = _clubService.IsClubSecretary(userRoleClub);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_IsClubManager_ReturnsSuccess()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 40 };

            // Act
            var result = _clubService.IsClubManager(userRoleClub);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_IsClubManager_ReturnsFail()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 40000 };

            // Act
            var result = _clubService.IsClubManager(userRoleClub);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_IsClubTrainer_ReturnsSuccess()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 30 };

            // Act
            var result = _clubService.IsClubTrainer(userRoleClub);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_IsClubTrainer_ReturnsFail()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 30000 };

            // Act
            var result = _clubService.IsClubTrainer(userRoleClub);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_IsClubStaff_ReturnsSuccess()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 30 };

            // Act
            var result = _clubService.IsClubStaff(userRoleClub);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_IsClubStaff_ReturnsFail()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 30000 };

            // Act
            var result = _clubService.IsClubStaff(userRoleClub);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_IsClubAthlete_ReturnsSuccess()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 20 };

            // Act
            var result = _clubService.IsClubAthlete(userRoleClub);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_IsClubAthlete_ReturnsFail()
        {
            // Arrange
            var userRoleClub = new UsersRoleClub { RoleId = 20000 };

            // Act
            var result = _clubService.IsClubAthlete(userRoleClub);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_IsClubSecretary_FN_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _clubService.IsClubSecretary("Test 2",1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_IsClubSecretary_FN_ReturnsFail()
        {
            // Arrange

            // Act
            var result = _clubService.IsClubSecretary("Test 2", 2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_IsClubMember_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _clubService.IsClubMember("Test 2", 2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_IsClubMember_ReturnsFail()
        {
            // Arrange

            // Act
            var result = _clubService.IsClubMember("Test 1", 2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_IsClubPartner_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _clubService.IsClubPartner("Test 4", 1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_IsClubPartner_ReturnsFail()
        {
            // Arrange

            // Act
            var result = _clubService.IsClubMember("Test 4", 1);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_ApproveCode_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _clubService.ApproveCode("12345612431");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_ApproveCode_ReturnsFail()
        {
            // Arrange

            // Act
            var result = _clubService.ApproveCode(null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ClubService_SendCodeEmail_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _clubService.SendCodeEmail(5,"a@gmail.com", 2);

            // Assert
            result.Should().BeAssignableTo<Task>();
        }

        [Fact]
        public async Task ClubService_GetModalities_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetModalities();

            // Assert
            result.Should().BeAssignableTo<List<Modality>>();
        }

        [Fact]
        public void ClubService_UserHasRoleInClub_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _clubService.UserHasRoleInClub("Test 4",1,10);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ClubService_UserHasRoleInClub_ReturnsFail()
        {
            // Arrange

            // Act
            var result = _clubService.UserHasRoleInClub("Test 3", 2, 10);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ClubService_GetClubPartners_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClubPartners(1);

            // Assert
            result.Should().BeOfType<List<UsersRoleClub>>();
        }
        
        [Fact]
        public async Task ClubService_RemoveClubUser_ReturnsSuccess()
        {
            // Arrange
            var count = _context.UsersRoleClub.Count();


            // Act
            await _clubService.AddPartnerToClub("Test 1", 10);
            await _clubService.RemoveClubUser(count+1);

            // Assert
            _context.UsersRoleClub.Count().Should().Be(count);
        }

        [Fact]
        public async Task ClubService_UpdateClubPhoto_ReturnsSuccess()
        {
            // Arrange
            var club = _context.Club.First();

            // Act
            await _clubService.UpdateClubPhoto(club, false, A.Fake<IFormFile>());

            // Assert
            club.Photography.Should().NotBeNull();
        }

        [Fact]
        public async Task ClubService_UpdateClubPhoto_ReturnsFileNull()
        {
            // Arrange
            var club = _context.Club.First();

            // Act
            await _clubService.UpdateClubPhoto(club, false, null);
            
            // Assert
            club.Photography.Should().BeNull();
        }

        [Fact]
        public async Task ClubService_UpdateClubPhoto_ReturnsRemove()
        {
            // Arrange
            var club = _context.Club.First();
            club.Photography = A.Fake<BlobDto>();

            // Act
            await _clubService.UpdateClubPhoto(club, true, null);

            // Assert
            club.Photography.Should().BeNull();
        }
        
        [Fact]
        public async Task ClubService_UpdateClubModalities_ReturnsSuccess()
        {
            // Arrange
            Club club = _context.Club.Where(c => c.Id == 1).Include(c => c.Modalities).First();
            var modalitiesIds = new List<int>() { 2, 3, 4 };
            
            // Act
            await _clubService.UpdateClubModalities(club, modalitiesIds);

            // Assert
            club.Modalities.Should().HaveCount(3);
        }

        [Fact]
        public async Task ClubService_GetUserRoleClubFromId_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetUserRoleClubFromId(1);

            // Assert
            result.Should().BeOfType<UsersRoleClub>();
        }

        [Fact]
        public async Task ClubService_CreateAddress_ReturnsSuccess()
        {
            // Arrange
            var address = new Address()
            {
                ZipCode = "222-22",
                Street = "Rua",
                City = "Lisboa",
                District = "Lisboa",
                Country = "Portugal",
            };

            // Act
            var result = await _clubService.CreateAddress(address, 1);

            // Assert
            result.Should().BeOfType<Address>();
        }

        [Fact]
        public async Task ClubService_GetClubStaff_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClubStaff(1);

            // Assert
            result.Should().BeOfType<List<UsersRoleClub>>();
        }

        [Fact]
        public async Task ClubService_GetClubAthletes_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClubAthletes(5);

            // Assert
            result.Should().BeOfType<List<UsersRoleClub>>();
        }

        [Fact]
        public async Task ClubService_GetClubModalities_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClubModalities(1);

            // Assert
            result.Should().BeOfType<List<Modality>>();
        }

        [Fact]
        public async Task ClubService_GetAthletes_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetAthletes(1);

            // Assert
            result.Should().BeOfType<List<User>>();
        }

        [Fact]
        public async Task ClubService_GetClubTrainers_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClubTrainers(5);

            // Assert
            result.Should().BeOfType<List<User>>();
        }

        [Fact]
        public async Task ClubService_GetClubTranslations_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClubTranslations(1);

            // Assert
            result.Should().BeOfType<List<ClubTranslations>>();
        }

        [Fact]
        public async Task ClubService_AddPartnerToClub_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.AddPartnerToClub("Test 8", 1, UserRoleStatus.Active);

            // Assert
            result.Should().BeOfType<UsersRoleClub>();
        }

        [Fact]
        public async Task ClubService_UpdateClubAddress_ReturnsSuccess()
        {
            // Arrange
            var address = new Address()
            {
                Id = 1,
                ZipCode = "222-22",
                Street = "Rua",
                City = "Lisboa",
                District = "Lisboa",
                Country = "Portugal",
            };

            // Act
            await _clubService.UpdateClubAddress(address, 1);

            // Assert
            _context.Address.Where(a => a.Id == 1).First().Street.Should().Be("Rua");
        }

        [Fact]
        public async Task ClubService_GetAllCoordinates_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetAllCoordinates();

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ClubService_SearchNameClubs_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.SearchNameClubs("Test Club");

            // Assert
            result.Should().BeAssignableTo<List<Club>>();
        }

        [Fact]
        public async Task ClubService_SearchNameClubs_ReturnsNameNull()
        {
            // Arrange
            
            // Act
            var result = await _clubService.SearchNameClubs(null);

            // Assert
            result.Should().BeOfType<List<Club>>();
        }

        [Fact]
        public async Task ClubService_GetClubStatus_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _clubService.GetClubStatus(1);

            // Assert
            result.Should().BeOneOf(ClubStatus.Active, ClubStatus.Waiting_Payment, ClubStatus.Suspended, ClubStatus.Deleted);
        }

        [Fact]
        public async Task ClubService_UpdateClubPaymentSettings_ReturnsSuccess()
        {
            // Arrange
            var settings = new ClubPaymentSettings()
            {
                ClubPaymentSettingsId = 1,
                AccountId = "Test Account",
                AccountKey = "Test Key",
                ValidCredentials = true,
            };

            // Act
            var result = await _clubService.UpdateClubPaymentSettings(settings);

            // Assert
            result.Should().BeOfType<ClubPaymentSettings>();
        }
    }
}
