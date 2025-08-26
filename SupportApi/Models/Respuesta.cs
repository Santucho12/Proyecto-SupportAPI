using System;

namespace SupportApi.Models
{
    public class Respuesta
    {
        public Guid Id { get; set; }  // Identificador único

        public string Contenido { get; set; } = null!;  // Texto de la respuesta

        public DateTime FechaRespuesta { get; set; } = DateTime.UtcNow;  // Fecha y hora en que se creó

        public bool Visto { get; set; } = false; // Indica si el usuario ya vio la respuesta

        // FK al reclamo al que pertenece esta respuesta
        public Guid ReclamoId { get; set; }

        public Reclamo? Reclamo { get; set; }

        // FK al usuario que realizó la respuesta (puede ser soporte o cliente)
        public Guid UsuarioId { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
