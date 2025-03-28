namespace ProgettoWebApi.DTOs.Evento
{
    public class CreateEventoRequestDto
    {
        public string Titolo { get; set; }
        public DateTime Data { get; set; }
        public string Luogo { get; set; }
        public int ArtistaId { get; set; }
        public int QuantitaBiglietti { get; set; }
    }
}
