using System;
using System.Collections.Generic;

namespace SupportApi.Models
{
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum EstadoReclamo
    {
        Nuevo,
        EnProceso,
        Respondido,
        Cerrado
    }

    public class Reclamo
    {
        public Guid Id { get; set; }  // Identificador único

        public string Titulo { get; set; } = null!;  // Título del reclamo

        public string Descripcion { get; set; } = null!;  // Descripción detallada

        public string Prioridad { get; set; } = "Baja";  // Prioridad del reclamo (Baja, Media, Alta)

        public EstadoReclamo Estado { get; set; } = EstadoReclamo.Nuevo;  // Estado del reclamo


        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;  // Fecha de creación

        // FK al usuario que creó el reclamo
        public Guid UsuarioId { get; set; }

        // Navegación hacia el usuario creador
        public Usuario? Usuario { get; set; }

        // Un reclamo puede tener muchas respuestas
        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}
