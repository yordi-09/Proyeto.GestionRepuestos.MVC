using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Proyeto.GestionRepuestos.MVC.Models
{
    // Para agregar datos de perfil del usuario, agregue más propiedades a su clase ApplicationUser. Visite https://go.microsoft.com/fwlink/?LinkID=317594 para obtener más información.
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenga en cuenta que authenticationType debe coincidir con el valor definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar reclamaciones de usuario personalizadas aquí
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Repuesto> Repuestos { get; set; }
        public DbSet<SolicitudRepuesto> SolicitudesRepuestos { get; set; }
        public DbSet<EntregaRepuesto> EntregasRepuestos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SolicitudRepuesto>()
                .HasRequired(s => s.UsuarioSolicitador)
                .WithMany()
                .HasForeignKey(s => s.UsuarioSolicitadorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EntregaRepuesto>()
                .HasRequired(e => e.UsuarioEntregador)
                .WithMany()
                .HasForeignKey(e => e.UsuarioEntregadorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Repuesto>().ToTable("Repuesto");

            modelBuilder.Entity<SolicitudRepuesto>().ToTable("SolicitudRepuesto");

            //modelBuilder.Entity<EntregaRepuesto>().ToTable("EntregaRepuesto");
        }
    }
}