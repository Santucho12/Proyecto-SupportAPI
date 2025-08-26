using System;

namespace SupportApi.DTOs
{
    public class NotificacionDto
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Mensaje { get; set; }
    }
}
