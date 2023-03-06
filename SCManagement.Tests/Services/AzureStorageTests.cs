using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.AzureStorageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Azure;
using System.Reflection;

namespace SCManagement.Tests.Services
{
    public class AzureStorageTests
    {
        private readonly AzureStorage _azureStorageService;

        public AzureStorageTests()
        {
            var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .Build();

            _azureStorageService = new AzureStorage(config, A.Fake<ILogger<AzureStorage>>());
        }

        [Fact]
        public async Task AzureStorage_UploadAsync_ReturnsSuccess()
        {
            // Arrange
            var blob = A.Fake<IFormFile>();

            // Act
            var result = await _azureStorageService.UploadAsync(blob);

            // Assert
            result.Should().BeOfType<BlobResponseDto>().Which.Error.Should().BeFalse();
            result.Should().BeOfType<BlobResponseDto>().Which.Status.Should().Contain("Uploaded Successfully");
        }

        [Fact]
        public async Task AzureStorage_DeleteAsync_ReturnsSuccess()
        {
            // Arrange
            var formFile = A.Fake<IFormFile>();

            // Act
            var result = await _azureStorageService.UploadAsync(formFile);
            var blob = result.Blob;
            result = await _azureStorageService.DeleteAsync(blob.Uuid);

            // Assert
            result.Should().BeOfType<BlobResponseDto>().Which.Error.Should().BeFalse();
            result.Should().BeOfType<BlobResponseDto>().Which.Status.Should().Contain("has been successfully deleted.");
        }

        [Fact]
        public async Task AzureStorage_DeleteAsync_ReturnsFail()
        {
            // Arrange

            // Act
            var result = await _azureStorageService.DeleteAsync("ola");

            // Assert
            result.Should().BeOfType<BlobResponseDto>().Which.Error.Should().BeTrue();
            result.Should().BeOfType<BlobResponseDto>().Which.Status.Should().Be($"File with name ola not found.");
        }
    }
}
