namespace SCManagement.Models
{
    public interface ITranslation
    {
        public int Id { get; set; }

        public string? Language { get; set; }

        public string? Value { get; set; }

        public string? Atribute { get; set; }
    }
}
