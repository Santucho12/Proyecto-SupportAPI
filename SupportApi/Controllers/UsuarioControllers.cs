using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportApi.Data;
using SupportApi.Models;

namespace SupportApi.Controllers
{
    /// <summary>Gestión de usuarios (solo Admin).</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Usuario,Admin,Soporte")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class UsuariosController : ControllerBase
    {
        private readonly SupportDbContext _context;

        public UsuariosController(SupportDbContext context)
        {
            _context = context;
        }

        // Endpoint temporal para actualizar el rol de un usuario por email
        // Elimina este endpoint después de usarlo por seguridad
        [HttpGet("actualizar-rol-temporal")]
        public async Task<IActionResult> ActualizarRolTemporal([FromQuery] string email, [FromQuery] string nuevoRol)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico == email);
            if (usuario == null)
                return NotFound("Usuario no encontrado");
            usuario.Rol = nuevoRol;
            await _context.SaveChangesAsync();
            return Ok($"Rol actualizado a {nuevoRol} para {email}");
        }

        // GET: api/usuarios
        // GET: api/usuarios
        /// <summary>Obtiene todos los usuarios registrados.</summary>
        /// <returns>Lista de usuarios</returns>
        /// <response code="200">OK. Devuelve la lista de usuarios.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/usuarios/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Usuario>> GetUsuario(Guid id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Reclamos)
                .Include(u => u.Respuestas)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound();

            return NoContent();
        }

        // DELETE: api/usuarios/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUsuario(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Usuario eliminado correctamente" });
        }
    }
}
