using Microsoft.EntityFrameworkCore;
using SupportApi.Data;
using SupportApi.Models;
namespace SupportApi.Repositories
{
    public class ReclamoRepository : IReclamoRepository
    {
        private readonly SupportDbContext _context;
        public ReclamoRepository(SupportDbContext context)
        {
            _context = context;
        }
        // Implementación de métodos CRUD
        public async Task<List<Reclamo>> ObtenerTodosAsync()
        {
            return await _context.Reclamos.Include(r => r.Usuario).Include(r => r.Respuestas).ToListAsync();
        }
        public async Task<IEnumerable<Reclamo>> GetAllAsync()
        {
            return await _context.Reclamos.Include(r => r.Usuario).Include(r => r.Respuestas).ToListAsync();
        }

        public async Task<Reclamo?> GetByIdAsync(Guid id)
        {
            return await _context.Reclamos.Include(r => r.Usuario).Include(r => r.Respuestas).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reclamo?> CreateAsync(Reclamo reclamo)
        {
            // Validación: no permitir reclamos duplicados por título y usuario
            var exists = await _context.Reclamos.AnyAsync(r => r.Titulo == reclamo.Titulo && r.UsuarioId == reclamo.UsuarioId);
            if (exists) return null;
            _context.Reclamos.Add(reclamo);
            await _context.SaveChangesAsync();
            return reclamo;
        }

        public async Task<bool> UpdateAsync(Reclamo reclamo)
        {
            var existing = await _context.Reclamos.FindAsync(reclamo.Id);
            if (existing == null) return false;
            // Validación de estado usando enum
            if (!Enum.IsDefined(typeof(Models.EstadoReclamo), reclamo.Estado)) return false;
            existing.Titulo = reclamo.Titulo;
            existing.Descripcion = reclamo.Descripcion;
            existing.Estado = reclamo.Estado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var reclamo = await _context.Reclamos.FindAsync(id);
            if (reclamo == null) return false;
            _context.Reclamos.Remove(reclamo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
