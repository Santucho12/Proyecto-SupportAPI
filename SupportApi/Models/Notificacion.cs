using System;

namespace SupportApi.Models
{
    public class Notificacion
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public string Mensaje { get; set; } = null!;
    }
}
