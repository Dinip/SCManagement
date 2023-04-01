using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.TeamService;
using SCManagement.Services.TranslationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCManagement.Tests.Services
{
    public class TranslationServiceTests
    {
        private readonly TranslationService _translationService;
        
        public TranslationServiceTests()
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .Build();

            //SUT (system under test)
            _translationService = new TranslationService(config);
        }

        [Fact]
        public void TranslationServiceTests_Translate_ReturnsSuccess()
        {
            // Arrange
            var list = new List<ITranslation>() 
            {
                new ClubTranslations{ Language = "en-US", Value = "Hello" },
                new ClubTranslations{ Language = "pt-PT", Value = "" },
            };

            // Act
            var result = _translationService.Translate(list);

            // Assert
            result.Should().Be(Task.CompletedTask);
            list.Should().NotBeNull();
            list.Should().HaveCount(2);
            list.Should().Contain(x => x.Value == "Olá");
        }
        
        [Fact]
        public void TranslationServiceTests_Translate_ReturnsTranslationsNull()
        {
            // Arrange

            // Act
            var result = _translationService.Translate(null);

            // Assert
            result.Should().Be(Task.CompletedTask);
        }

        [Fact]
        public void TranslationServiceTests_Translate_ReturnsTranslationNull()
        {
            // Arrange
            var list = new List<ITranslation>()
            {
                new ClubTranslations{ Language = "en-US", Value = "" },
                new ClubTranslations{ Language = "pt-PT", Value = "" },
            };

            // Act
            var result = _translationService.Translate(list);

            // Assert
            result.Should().Be(Task.CompletedTask);
        }

        [Fact]
        public async Task TranslationServiceTests_Translation_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _translationService.Translation("Hello", "en-US", "pt-PT");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }
    }
}
