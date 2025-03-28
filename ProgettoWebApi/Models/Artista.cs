using System.ComponentModel.DataAnnotations;

namespace ProgettoWebApi.Models
{
    public class Artista
    {
        [Key]
        public int ArtistaId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nome { get; set; }

        [Required]
        [StringLength(50)]
        public string Genere { get; set; }

        [Required]
        [StringLength(1000)]
        public string Biografia { get; set; }

        public ICollection<Evento> Eventi { get; set; }
    }
}
