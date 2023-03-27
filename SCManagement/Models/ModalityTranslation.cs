namespace SCManagement.Models
{
    public class ModalityTranslation : ITranslation
    {
        public int Id { get; set; }
        public int ModalityId { get; set; }
        public Modality? Modality { get; set; }
        public string? Language { get; set; }
        public string? Value { get; set; }
        public string? Atribute { get; set; }
    }
}
