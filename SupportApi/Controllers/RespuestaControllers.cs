using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportApi.Data;
using SupportApi.Models;

namespace SupportApi.Controllers
{
    /// <summary>Gestión de respuestas a reclamos (Soporte/Admin).</summary>
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class RespuestasController : ControllerBase
    {

        private readonly Services.IRespuestaService _respuestaService;
        private readonly SupportDbContext _context;

        public RespuestasController(SupportDbContext context, Services.IRespuestaService respuestaService)
        {
            _context = context;
            _respuestaService = respuestaService;
        }
        /// <summary>
        /// Obtiene las respuestas no vistas (notificaciones) para un usuario.
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de respuestas no vistas</returns>
        /// <response code="200">OK. Devuelve la lista de respuestas no vistas.</response>
        [HttpGet("notificaciones")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<IEnumerable<Respuesta>>> GetNotificaciones([FromQuery] Guid usuarioId)
        {
            // Solo el usuario dueño puede ver sus notificaciones
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId) || userId != usuarioId)
            {
                return Forbid();
            }
            var noVistas = await _respuestaService.ObtenerNoVistasAsync(usuarioId);
            return Ok(noVistas);
        }

        /// <summary>
        /// Marca una respuesta como vista.
        /// </summary>
        /// <param name="id">ID de la respuesta</param>
        /// <response code="204">Marcado correctamente</response>
        /// <response code="404">No encontrada</response>
        [HttpPatch("{id:guid}/visto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarcarComoVisto(Guid id)
        {
            var respuesta = await _context.Respuestas.FindAsync(id);
            if (respuesta == null) return NotFound();
            await _respuestaService.MarcarComoVistoAsync(id);
            return NoContent();
        }

        // GET: api/respuestas
        // Permite opcionalmente filtrar por ReclamoId: /api/respuestas?reclamoId=...
        /// <summary>Obtiene todas las respuestas, opcionalmente filtradas por reclamo.</summary>
        /// <param name="reclamoId">ID del reclamo (opcional)</param>
        /// <returns>Lista de respuestas</returns>
        /// <response code="200">OK. Devuelve la lista de respuestas.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Respuesta>>> GetRespuestas([FromQuery] Guid? reclamoId)
        {
            var query = _context.Respuestas
                .Include(r => r.Usuario)
                .Include(r => r.Reclamo)
                .AsQueryable();

            if (reclamoId.HasValue)
                query = query.Where(r => r.ReclamoId == reclamoId.Value);

            return await query.AsNoTracking().ToListAsync();
        }

        // GET: api/respuestas/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Respuesta>> GetRespuesta(Guid id)
        {
            var respuesta = await _context.Respuestas
                .Include(r => r.Usuario)
                .Include(r => r.Reclamo)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (respuesta == null) return NotFound();

            return respuesta;
        }

        // POST: api/respuestas
        /// <summary>Crea una respuesta a un reclamo.</summary>
        /// <param name="respuesta">Datos de la respuesta</param>
        /// <returns>Respuesta creada</returns>
        /// <response code="201">Respuesta creada correctamente</response>
        /// <response code="400">Datos inválidos</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Respuesta>> CreateRespuesta(Respuesta respuesta)
        {
            // Validaciones mínimas para evitar FK inválidas
            var reclamoExists = await _context.Reclamos.AnyAsync(r => r.Id == respuesta.ReclamoId);
            if (!reclamoExists) return BadRequest("El ReclamoId no existe.");

            var usuarioExists = await _context.Usuarios.AnyAsync(u => u.Id == respuesta.UsuarioId);
            if (!usuarioExists) return BadRequest("El UsuarioId no existe.");

            respuesta.Id = Guid.NewGuid();
            respuesta.FechaRespuesta = DateTime.UtcNow;

            // Usar el servicio para crear la respuesta y disparar la notificación
            var nuevaRespuesta = await _respuestaService.CrearRespuestaAsync(respuesta);

            return CreatedAtAction(nameof(GetRespuesta), new { id = nuevaRespuesta.Id }, nuevaRespuesta);
        }

        // PUT: api/respuestas/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRespuesta(Guid id, Respuesta respuesta)
        {
            if (id != respuesta.Id) return BadRequest("El id del path no coincide con el del cuerpo.");

            var exists = await _context.Respuestas.AnyAsync(r => r.Id == id);
            if (!exists) return NotFound();

            // Opcional: bloquear cambios de ReclamoId/UsuarioId si no querés permitirlos
            _context.Entry(respuesta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Respuestas.AnyAsync(r => r.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/respuestas/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRespuesta(Guid id)
        {
            var respuesta = await _context.Respuestas.FindAsync(id);
            if (respuesta == null) return NotFound();

            _context.Respuestas.Remove(respuesta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
