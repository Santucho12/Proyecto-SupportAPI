using System;
using System.Collections.Generic;

namespace SupportApi.DTOs
{
    /// <summary>DTO para reclamos.</summary>
    /// <example>
    /// {
    ///   "id": "00000000-0000-0000-0000-000000000000",
    ///   "titulo": "No funciona el producto",
    ///   "descripcion": "El producto dejó de funcionar a los 2 días.",
    ///   "estado": "Nuevo",
    ///   "fechaCreacion": "2025-07-29T12:00:00Z",
    ///   "usuarioId": "00000000-0000-0000-0000-000000000000",
    ///   "usuarioNombre": "Juan Pérez",
    ///   "respuestas": []
    /// }
    /// </example>
    public class ReclamoDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Prioridad { get; set; } = "Baja";
        public string Estado { get; set; } = "Abierto";
        public DateTime FechaCreacion { get; set; }
        public Guid UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public List<RespuestaDto>? Respuestas { get; set; }


    }

    /// <summary>DTO para respuestas a reclamos.</summary>
    /// <example>
    /// {
    ///   "id": "00000000-0000-0000-0000-000000000000",
    ///   "contenido": "Estamos revisando su caso.",
    ///   "fechaCreacion": "2025-07-29T12:00:00Z",
    ///   "usuarioId": "00000000-0000-0000-0000-000000000000",
    ///   "usuarioNombre": "Soporte"
    /// }
    /// </example>
    public class RespuestaDto
    {
    public Guid Id { get; set; }
    public string Contenido { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public Guid UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }
    public DateTime FechaRespuesta { get; set; } // Para compatibilidad con el controlador
    public Guid ReclamoId { get; set; } // Para compatibilidad con el controlador
    }
}
