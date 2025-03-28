namespace ProgettoWebApi.DTOs.Evento
{
    public class EventoResponseDto
    {
        public int EventoId { get; set; }
        public string Titolo { get; set; }
        public DateTime Data { get; set; }
        public string Luogo { get; set; }
        public int ArtistaId { get; set; }
        public string ArtistaNome { get; set; }
    }
}
