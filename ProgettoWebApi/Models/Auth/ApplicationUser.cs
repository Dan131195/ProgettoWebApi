using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProgettoWebApi.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public required string Nome { get; set; }

        [Required]
        public required string Cognome { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
        public ICollection<Biglietto> Biglietti { get; set; }
    }
}
