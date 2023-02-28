using FakeItEasy;
using FluentAssertions;
using SCManagement.Controllers;
using SCManagement.Services.TranslationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCManagement.Tests.Controller
{
    public  class TranslationControllerTests
    {
        private readonly ITranslationService _translationService;
        private readonly TranslationController _translationController;

        public TranslationControllerTests()
        {
            _translationService = A.Fake<ITranslationService>();

            //SUT (system under test)
            _translationController = new TranslationController(_translationService);
        }


        [Fact]
        public async Task Translation()
        {

            // Arrange
            var list = new List<TranslationsContainer>
            {
                new TranslationsContainer
                {
                    Translations = new Translation[1]{ new Translation() { Text = "Olá", To = "pt" } }
                }
            };
            A.CallTo(() => _translationService.Translation(A<string>._, A<string>._, A<string>._)).Returns(list);


            // Act
            var result = await _translationController.Translation("Hello", "en-US", "pt-PT");

            // Assert
            result.Should().BeOfType<List<TranslationsContainer>>().And.NotBeNull();
           
        }
    }
}
