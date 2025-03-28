using System.ComponentModel.DataAnnotations;

namespace ProgettoWebApi.Models
{
    public class Evento
    {
        [Key]
        public int EventoId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Titolo { get; set; }

        [Required]
        public DateTime Data {  get; set; }

        [Required]
        [StringLength(100)]
        public string Luogo { get; set; }

        public int ArtistaId { get; set; }
        public Artista Artista { get; set; }

        public ICollection<Biglietto> Biglietti { get; set; }
    }
}
