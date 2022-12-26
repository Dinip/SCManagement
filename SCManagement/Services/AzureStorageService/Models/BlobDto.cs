using System.ComponentModel.DataAnnotations.Schema;

namespace SCManagement.Services.AzureStorageService.Models
{
    public class BlobDto
    {
        public int Id { get; set; }
        public string Uuid { get; set; }
        public string? OriginalName { get; set; }
        public string? ContentType { get; set; }
        public string? Uri { get; set; }
        [NotMapped]
        public Stream? Content { get; set; }
        public DateTime CreatedAt { get; } = new DateTime();
    }
}
