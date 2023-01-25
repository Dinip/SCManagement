using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using SCManagement.Services.AzureStorageService.Models;
using Azure;

namespace SCManagement.Services.AzureStorageService
{
    public class AzureStorage : IAzureStorage
    {
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private readonly string _cdnUrl;
        private readonly ILogger<AzureStorage> _logger;

        public AzureStorage(IConfiguration configuration, ILogger<AzureStorage> logger)
        {
            _storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
            _storageContainerName = configuration.GetValue<string>("BlobContainerName");
            _cdnUrl = configuration.GetValue<string>("CdnUrl");
            _logger = logger;
        }

        /// <summary>
        /// Handles the upload of a file to Azure Blob Storage
        /// </summary>
        /// <param name="blob"></param>
        /// <returns>An object with the information and url to the file</returns>
        public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
        {
            // Create new upload response object that we can return to the requesting method
            BlobResponseDto response = new();

            // Get a reference to a container named in appsettings.json and then create it
            BlobContainerClient container = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            //await container.CreateAsync();
            string uuid = Guid.NewGuid().ToString();
            try
            {
                var filenameArray = blob.FileName.Split(".");
                uuid = uuid + "." + filenameArray[^1];

                // Get a reference to the blob just uploaded from the API in a container from configuration settings
                BlobClient client = container.GetBlobClient(uuid.ToString());

                // Open a stream for the file we want to upload
                await using (Stream? data = blob.OpenReadStream())
                {
                    var blobHttpHeader = new BlobHttpHeaders { ContentType = blob.ContentType };
                    // Upload the file async
                    await client.UploadAsync(data, new BlobUploadOptions { HttpHeaders = blobHttpHeader });
                }

                // Everything is OK and file got uploaded
                response.Status = $"File {blob.FileName}/{uuid} Uploaded Successfully";
                response.Error = false;
                response.Blob.Uri = $"{_cdnUrl}/uploads/{uuid}";
                response.Blob.OriginalName = blob.FileName;
                response.Blob.Uuid = uuid;
                response.Blob.ContentType = blob.ContentType;

            }
            // If the file already exists, we catch the exception and do not upload it
            catch (RequestFailedException ex)
               when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                _logger.LogError($"File with name {blob.FileName}/{uuid} already exists in container. Set another name to store the file in the container: '{_storageContainerName}.'");
                response.Status = $"File with name {blob.FileName}/{uuid} already exists. Please use another name to store your file.";
                response.Error = true;
                return response;
            }
            // If we get an unexpected error, we catch it here and return the error message
            catch (RequestFailedException ex)
            {
                // Log error to console and create a new response we can return to the requesting method
                _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
                response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
                response.Error = true;
                return response;
            }

            // Return the BlobUploadResponse object
            return response;
        }

        /// <summary>
        /// Deletes a file from Azure Blob Storage
        /// </summary>
        /// <param name="FileUUID"></param>
        /// <returns>An object with the deleted information</returns>
        public async Task<BlobResponseDto> DeleteAsync(string FileUUID)
        {
            BlobContainerClient client = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            BlobClient file = client.GetBlobClient(FileUUID.ToString());

            try
            {
                // Delete the file
                await file.DeleteAsync();
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                // File did not exist, log to console and return new response to requesting method
                _logger.LogError($"File {FileUUID} was not found.");
                return new BlobResponseDto { Error = true, Status = $"File with name {FileUUID} not found." };
            }

            // Return a new BlobResponseDto to the requesting method
            return new BlobResponseDto { Error = false, Status = $"File: {FileUUID} has been successfully deleted." };
        }
    }
}
