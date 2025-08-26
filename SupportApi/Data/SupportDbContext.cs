using Microsoft.EntityFrameworkCore;
using SupportApi.Models;

namespace SupportApi.Data
{
    public class SupportDbContext : DbContext
    {
        public SupportDbContext(DbContextOptions<SupportDbContext> options)
            : base(options)
        {
        }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Reclamo> Reclamos { get; set; }
    public DbSet<Respuesta> Respuestas { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración del enum EstadoReclamo para almacenarlo como string
            modelBuilder.Entity<Reclamo>()
                .Property(r => r.Estado)
                .HasConversion<string>();

            // Configuración para evitar problemas con cascada múltiple en SQL Server
            modelBuilder.Entity<Respuesta>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Respuestas)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Respuesta>()
                .HasOne(r => r.Reclamo)
                .WithMany(rcl => rcl.Respuestas)
                .HasForeignKey(r => r.ReclamoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reclamo>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Reclamos)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
