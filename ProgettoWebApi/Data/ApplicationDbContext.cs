using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgettoWebApi.Models;
using ProgettoWebApi.Models.Auth;

namespace ProgettoWebApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Artista> Artisti { get; set; }
        public DbSet<Evento> Eventi { get; set; }
        public DbSet<Biglietto> Biglietti { get; set; }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Artista>()
                .HasMany(a => a.Eventi)
                .WithOne(e => e.Artista)
                .HasForeignKey(e => e.ArtistaId);

            builder.Entity<Evento>()
                .HasOne(e => e.Artista)
                .WithMany()
                .HasForeignKey(e => e.ArtistaId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Biglietto>()
                .HasOne(b => b.Evento)
                .WithMany(e => e.Biglietti)
                .HasForeignKey(b => b.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Biglietti)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

            builder.Entity<ApplicationUserRole>().HasOne(a => a.ApplicationUser).WithMany(u => u.UserRoles).HasForeignKey(a => a.UserId);

            builder.Entity<ApplicationUserRole>().HasOne(a => a.ApplicationRole).WithMany(r => r.UserRoles).HasForeignKey(a => a.RoleId);

            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = "1", Name = "Admin", NormalizedName = "SUPERADMIN" },
                new ApplicationRole { Id = "2", Name = "User", NormalizedName = "USER" }
            );
        }
    }
}
