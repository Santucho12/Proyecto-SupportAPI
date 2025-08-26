using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SupportApi.Data
{
    public class SupportDbContextFactory : IDesignTimeDbContextFactory<SupportDbContext>
    {
        public SupportDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SupportDbContext>();

            // Aquí pones tu cadena de conexión (igual que en appsettings.json)
            optionsBuilder.UseSqlServer("Server=db;Database=SupportDB;User=sa;Password=Your_password123;TrustServerCertificate=True");

            return new SupportDbContext(optionsBuilder.Options);
        }
    }
}
