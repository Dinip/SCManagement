using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SCManagement.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCManagement.Tests.Controller
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly ILogger<HomeController> _logger;

        public HomeControllerTests()
        {
            _logger = A.Fake<ILogger<HomeController>>();

            //SUT (system under test)
            _controller = new HomeController(_logger);
        }

        [Fact]
        public void HomeController_Index_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void HomeController_About_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _controller.About();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void HomeController_Settings_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _controller.Settings();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }
}
