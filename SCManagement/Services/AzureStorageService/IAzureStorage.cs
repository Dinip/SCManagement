using SCManagement.Services.AzureStorageService.Models;

namespace SCManagement.Services.AzureStorageService
{
    public interface IAzureStorage
    {
        /// <summary>
        /// This method uploads a file submitted with the request
        /// </summary>
        /// <param name="file">File for upload</param>
        /// <returns>Blob with status</returns>
        Task<BlobResponseDto> UploadAsync(IFormFile file);

        /// <summary>
        /// This method deleted a file with the specified filename
        /// </summary>
        /// <param name="FileUUID">File uuid</param>
        /// <returns>Blob with status</returns>
        Task<BlobResponseDto> DeleteAsync(string FileUUID);
    }
}
