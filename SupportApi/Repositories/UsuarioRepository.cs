using SupportApi.Models;
using Microsoft.EntityFrameworkCore;
namespace SupportApi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SupportApi.Data.SupportDbContext _context;
        public UsuarioRepository(SupportApi.Data.SupportDbContext context)
        {
            _context = context;
        }
        // Implementación de métodos CRUD
        public async Task<Usuario?> CreateAsync(Usuario usuario)
        {
            // Validación: no permitir usuarios duplicados por email
            var exists = await _context.Usuarios.AnyAsync(u => u.CorreoElectronico == usuario.CorreoElectronico);
            if (exists) return null;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
        public async Task<int> ContarAsync()
        {
            return await _context.Usuarios.CountAsync();
        }
    }
}
