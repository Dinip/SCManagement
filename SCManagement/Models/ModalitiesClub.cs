namespace SCManagement.Models
{
    /// <summary>
    /// This class represents a Modalities of a Club
    /// </summary>
    public class ModalitiesClub
    {
        public int Id { get; set; }

        public int ClubId { get; set; }

        public Club? club { get; set; }

        public int ModalityId{ get; set; }

        public Modality? Modality { get; set; }
    }
}
