using System.ComponentModel.DataAnnotations;
using ProgettoWebApi.Models.Auth;

namespace ProgettoWebApi.Models
{
    public class Biglietto
    {
        [Required]
        public int BigliettoId { get; set; }
        public int EventoId { get; set; }
        public Evento Evento { get; set; }

        public int ArtistaId { get; set; }
        public Artista Artista { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public DateTime DataAcquisto { get; set; }
    }
}
