namespace ProgettoWebApi.DTOs.Biglietto
{
    public class BigliettoResponseDto
    {
        public int BigliettoId { get; set; }
        public int EventoId { get; set; }
        public string? EventoTitolo { get; set; }
        public DateTime? DataEvento { get; set; }

        public int ArtistaId { get; set; }
        public string? ArtistaNome { get; set; }

        public string? UserId { get; set; }
        public string? EmailUtente { get; set; }

        public DateTime DataAcquisto { get; set; }
    }

}
