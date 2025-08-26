using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using SupportApi.Models;
using SupportApi.Data;
using SupportApi.DTOs;

namespace SupportApi.Controllers
{
    [ApiController]
    [Route("api/notificaciones")]
    public class NotificacionesControllers : ControllerBase
    {
        private readonly SupportDbContext _context;

        public NotificacionesControllers(SupportDbContext context)
        {
            _context = context;
        }

        // Endpoint para soporte: devuelve todas las notificaciones relevantes
        [HttpGet("soporte")]
        public ActionResult<IEnumerable<NotificacionDto>> GetNotificacionesSoporte()
        {
            // Ejemplo: todas las notificaciones de tipo reclamo, respuesta o estado
            var notificaciones = _context.Notificaciones
                .Where(n => n.Tipo == "reclamo" || n.Tipo == "respuesta" || n.Tipo == "estado")
                .OrderByDescending(n => n.FechaCreacion)
                .ToList();
            return Ok(notificaciones);
        }
    }
}
