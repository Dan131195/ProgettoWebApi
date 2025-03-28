namespace ProgettoWebApi.DTOs.Biglietto
{
    public class BigliettoResponseDto
    {
        public int BigliettoId { get; set; }
        public int EventoId { get; set; }
        public string TitoloEvento { get; set; }
        public DateTime DataEvento { get; set; }
        public DateTime DataAcquisto { get; set; }
        public string ArtistaNome { get; set; }
    }
}
