using SupportApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using SupportApi.Models;
using SupportApi.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using SupportApi.Validators;

namespace SupportApi.Controllers
{
    /// <summary>Gestión de reclamos. Usuario puede crear, Soporte/Admin pueden gestionar.</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class ReclamosController : ControllerBase
    {
        private readonly SupportDbContext _context;
        private readonly IValidator<ReclamoDto> _validator;
        public ReclamosController(SupportDbContext context, IValidator<ReclamoDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        // GET: api/reclamos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTOs.ReclamoDto>>> ObtenerTodos()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type.EndsWith("nameidentifier"));
            if (userIdClaim == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdClaim.Value, out Guid usuarioId))
                return Unauthorized();

            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role || c.Type == "role" || c.Type.EndsWith("/role"));
            var rol = rolClaim?.Value ?? "Usuario";

            List<Reclamo> reclamos;
            if (rol == "Soporte" || rol == "Admin")
            {
                reclamos = _context.Reclamos.OrderByDescending(r => r.FechaCreacion).ToList();
            }
            else
            {
                reclamos = _context.Reclamos
                    .Where(r => r.UsuarioId == usuarioId)
                    .OrderByDescending(r => r.FechaCreacion)
                    .ToList();
            }

            var reclamosDto = reclamos.Select(r => new DTOs.ReclamoDto {
                Id = r.Id,
                Titulo = r.Titulo,
                Descripcion = r.Descripcion,
                Prioridad = r.Prioridad,
                Estado = r.Estado.ToString(),
                FechaCreacion = r.FechaCreacion,
                UsuarioId = r.UsuarioId,
                Respuestas = null
            }).ToList();

            return Ok(reclamosDto);
        }

        // GET: api/reclamos/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Reclamo>> ObtenerPorId(Guid id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type.EndsWith("nameidentifier"));
            if (userIdClaim == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdClaim.Value, out Guid usuarioId))
                return Unauthorized();

            var reclamo = await _context.Reclamos.Include(r => r.Respuestas).FirstOrDefaultAsync(r => r.Id == id);
            if (reclamo == null)
                return NotFound();

            var reclamoDto = new DTOs.ReclamoDto {
                Id = reclamo.Id,
                Titulo = reclamo.Titulo,
                Descripcion = reclamo.Descripcion,
                Prioridad = reclamo.Prioridad,
                Estado = reclamo.Estado.ToString(),
                FechaCreacion = reclamo.FechaCreacion,
                UsuarioId = reclamo.UsuarioId,
                Respuestas = reclamo.Respuestas?.Select(rsp => new DTOs.RespuestaDto {
                    Id = rsp.Id,
                    Contenido = rsp.Contenido,
                    FechaRespuesta = rsp.FechaRespuesta,
                    UsuarioId = rsp.UsuarioId,
                    ReclamoId = rsp.ReclamoId
                }).ToList() ?? new List<DTOs.RespuestaDto>()
            };
            return Ok(reclamoDto);
        }

        // POST: api/reclamos
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Usuario")]
        public async Task<ActionResult<ReclamoDto>> Crear([FromBody] CreateReclamoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Forbid("No se pudo obtener el usuario autenticado.");

            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
                return BadRequest(new { Message = "El usuario autenticado no existe en la base de datos." });

            var reclamo = new Reclamo
            {
                Id = Guid.NewGuid(),
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Prioridad = dto.Prioridad,
                Estado = SupportApi.Models.EstadoReclamo.Nuevo,
                FechaCreacion = DateTime.Now,
                UsuarioId = userId
            };

                _context.Reclamos.Add(reclamo);
                await _context.SaveChangesAsync();

                // Notificar por email al usuario (nuevo reclamo creado)
                try {
                    var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                    var sendGridClient = new SendGrid.SendGridClient(sendGridApiKey);
                    var from = new SendGrid.Helpers.Mail.EmailAddress("soporte@tusistema.com", "Soporte Reclamos");
                    var to = new SendGrid.Helpers.Mail.EmailAddress(usuario.CorreoElectronico);
                    var subject = "Tu reclamo ha sido creado";
                    var plainTextContent = $"Hola {usuario.Nombre}, tu reclamo '{reclamo.Titulo}' ha sido registrado.";
                    var htmlContent = $"<strong>Hola {usuario.Nombre}, tu reclamo '{reclamo.Titulo}' ha sido registrado.</strong>";
                    var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    await sendGridClient.SendEmailAsync(msg);
                } catch {}
                // Notificar por WhatsApp al usuario (nuevo reclamo creado)
                try {
                    var twilioAccountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
                    var twilioAuthToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
                    Twilio.TwilioClient.Init(twilioAccountSid, twilioAuthToken);
                    var message = await Twilio.Rest.Api.V2010.Account.MessageResource.CreateAsync(
                        body: $"Hola {usuario.Nombre}, tu reclamo '{reclamo.Titulo}' ha sido registrado.",
                        from: new Twilio.Types.PhoneNumber("whatsapp:+14155238886"),
                        to: new Twilio.Types.PhoneNumber("whatsapp:+5492914405674")
                    );
                } catch (Exception ex) {
                    Console.WriteLine($"Twilio error (crear reclamo): {ex.Message}");
                }

                var reclamoDto = new ReclamoDto
                {
                    Id = reclamo.Id,
                    Titulo = reclamo.Titulo,
                    Descripcion = reclamo.Descripcion,
                    Prioridad = reclamo.Prioridad,
                    Estado = reclamo.Estado.ToString(),
                    FechaCreacion = reclamo.FechaCreacion,
                    UsuarioId = reclamo.UsuarioId,
                    Respuestas = new List<RespuestaDto>()
                };

                return CreatedAtAction(nameof(ObtenerPorId), new { id = reclamo.Id }, reclamoDto);
        }

        // PATCH: api/reclamos/{id}/estado
        [HttpPatch("{id}/estado")]
    // [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Soporte,Admin")]
        public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] SupportApi.DTOs.EstadoDto dto)
        {
            var reclamo = await _context.Reclamos.FindAsync(id);
            if (reclamo == null)
                return NotFound();

            if (string.IsNullOrEmpty(dto.Estado))
                return BadRequest("No se recibió el estado");

            if (!Enum.TryParse<SupportApi.Models.EstadoReclamo>(dto.Estado, out var nuevoEstado))
                return BadRequest("Estado inválido");

            reclamo.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/reclamos/{id}/actualizar
        [HttpPut("{id}/actualizar")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Soporte,Admin")]
        public async Task<IActionResult> Actualizar(Guid id, Reclamo reclamoActualizado)
        {
            if (id != reclamoActualizado.Id)
                return BadRequest("El ID del reclamo no coincide.");

            var reclamoExistente = await _context.Reclamos.FindAsync(id);
            if (reclamoExistente == null)
                return NotFound();

            reclamoExistente.Titulo = reclamoActualizado.Titulo;
            reclamoExistente.Descripcion = reclamoActualizado.Descripcion;
            reclamoExistente.Estado = reclamoActualizado.Estado;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/reclamos/{id}
        [HttpDelete("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Soporte,Admin")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var reclamo = await _context.Reclamos.FindAsync(id);
            if (reclamo == null)
                return NotFound();

            _context.Reclamos.Remove(reclamo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
