namespace SCManagement.Models
{
    public class ClubTranslations
    {
        public int Id { get; set; }

        public int ClubId { get; set; }

        public Club? Club { get; set; }
        
        //public string? PTText { get; set; }
        
        //public string? ENText { get; set; }

        public string? Language { get; set; }

        public string? value { get; set; }
        
        public string? Atribute { get; set; }
    }
}
