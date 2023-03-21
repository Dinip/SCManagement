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
using SCManagement.Services.AzureStorageService;
using Microsoft.Extensions.Localization;
using SCManagement.Services.ClubService;
using SCManagement.Services.StatisticsService;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.StatisticsService.Models;

namespace SCManagement.Tests.Services {
    public class StatisticsServiceTests {
        private readonly ApplicationDbContext _context;
        private readonly StatisticsService _statisticsService;
        private readonly IClubService _clubService;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public StatisticsServiceTests()
        {
            _context = GetDbContext().Result;
            _clubService = A.Fake<IClubService>();
            _stringLocalizer = A.Fake<IStringLocalizer<SharedResource>>();

            //SUT (system under test)
            _statisticsService = new StatisticsService(
                _context,
                _clubService,
                _stringLocalizer
             );
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SCManagementStatisticsService")
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            if (!await context.ClubPaymentStatistics.AnyAsync())
            {
                context.ClubPaymentStatistics.Add(new ClubPaymentStatistics
                {
                    Id = 1,
                    Value = 50, // 5x entradas para o evento 1 * 10€
                    ClubId = 1,
                    ProductId = 1,
                    ProductType = ProductType.Event,
                    StatisticsRange = StatisticsRange.Month,
                    Timestamp = new DateTime(2023, 03, 31),
                });

                context.ClubPaymentStatistics.Add(new ClubPaymentStatistics
                {
                    Id = 2,
                    Value = 999.5f, //50 socios * 19.99€
                    ClubId = 1,
                    ProductId = 2,
                    ProductType = ProductType.ClubMembership,
                    StatisticsRange = StatisticsRange.Month,
                    Timestamp = new DateTime(2023, 03, 31),
                });

                context.ClubPaymentStatistics.Add(new ClubPaymentStatistics
                {
                    Id = 3,
                    Value = 150, // 10x entradas para o evento 2 * 15€
                    ClubId = 1,
                    ProductId = 1,
                    ProductType = ProductType.Event,
                    StatisticsRange = StatisticsRange.Month,
                    Timestamp = new DateTime(2023, 03, 31),
                });

                context.ClubPaymentStatistics.Add(new ClubPaymentStatistics
                {
                    Id = 4,
                    Value = 200, // 5x + 10x entradas para os eventos (5 * 10 + 10 * 15)
                    ClubId = 1,
                    ProductId = null,
                    ProductType = ProductType.Event,
                    StatisticsRange = StatisticsRange.Month,
                    Timestamp = new DateTime(2023, 03, 31),
                });

                context.ClubPaymentStatistics.Add(new ClubPaymentStatistics
                {
                    Id = 5,
                    Value = 999.5f, //50 socios * 19.99€
                    ClubId = 1,
                    ProductId = null,
                    ProductType = ProductType.ClubMembership,
                    StatisticsRange = StatisticsRange.Month,
                    Timestamp = new DateTime(2023, 03, 31),
                });

                await context.SaveChangesAsync();
            }

            if (!await context.ClubUserStatistics.AnyAsync())
            {
                context.ClubUserStatistics.Add(new ClubUserStatistics
                {
                    Id = 1,
                    Value = 4, // 4x socios
                    RoleId = 10,
                    ClubId = 1,
                    StatisticsRange = StatisticsRange.Month,
                    Timestamp = new DateTime(2023, 03, 31),
                });

                context.ClubUserStatistics.Add(new ClubUserStatistics
                {
                    Id = 2,
                    Value = 5, // 5x atletas
                    RoleId = 20,
                    ClubId = 1,
                    StatisticsRange = StatisticsRange.Month,
                    Timestamp = new DateTime(2023, 03, 31),
                });

                await context.SaveChangesAsync();
            }

            if (!await context.ClubModalityStatistics.AnyAsync())
            {
                context.ClubModalityStatistics.Add(new ClubModalityStatistics
                {
                    Id = 1,
                    Value = 3, // 3x atletas football
                    ModalityId = 3,
                    ClubId = 1,
                    StatisticsRange = StatisticsRange.Month,
                    Timestamp = new DateTime(2023, 03, 31),
                });

                context.ClubModalityStatistics.Add(new ClubModalityStatistics
                {
                    Id = 2,
                    Value = 2, // 2x atletas futsal
                    ModalityId = 4,
                    ClubId = 1,
                    StatisticsRange = StatisticsRange.Month,
                    Timestamp = new DateTime(2023, 03, 31),
                });

                await context.SaveChangesAsync();
            }

            return context;
        }

        [Fact]
        public async Task StatisticsService_GetClubPaymentStatistics_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _statisticsService.GetClubPaymentStatistics(1, 2023);

            // Assert
            result.Should().HaveCount(2).And.AllBeOfType<ClubPaymentStatistics>();
        }

        [Fact]
        public async Task StatisticsService_GetClubPaymentDetailsStatistics_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _statisticsService.GetClubPaymentDetailsStatistics(1, 2023);

            // Assert
            result.Should().HaveCount(3).And.AllBeOfType<ClubPaymentStatistics>();
        }


        [Fact]
        public async Task StatisticsService_GetClubUserStatistics_Partners_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _statisticsService.GetClubUserStatistics(1, 10, 2023);

            // Assert
            result.Should().HaveCount(1).And.AllBeOfType<ClubUserStatistics>();
        }

        [Fact]
        public async Task StatisticsService_GetClubUserStatistics_Athletes_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _statisticsService.GetClubUserStatistics(1, 20, 2023);

            // Assert
            result.Should().HaveCount(1).And.AllBeOfType<ClubUserStatistics>();
        }

        [Fact]
        public async Task StatisticsService_GetClubModalityStatistics_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _statisticsService.GetClubModalityStatistics(1, 2023);

            // Assert
            result.Should().HaveCount(2).And.AllBeOfType<ClubModalityStatistics>();
        }
    }
}
