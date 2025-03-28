using Microsoft.AspNetCore.Identity;

namespace ProgettoWebApi.Models.Auth
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
