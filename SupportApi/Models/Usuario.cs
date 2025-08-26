using System;
using System.Collections.Generic;

namespace SupportApi.Models
{
    public enum RolUsuario
    {
        Usuario,
        Soporte,
        Admin
    }

    public class Usuario
    {
        public Guid Id { get; set; }  // Identificador único


        public string Nombre { get; set; } = null!;  // Nombre completo del usuario
        public string CorreoElectronico { get; set; } = null!;  // Email
        public string HashContrasena { get; set; } = null!;  // Contrase hasheada
        public string Rol { get; set; } = "Usuario";  // Rol del usuario

        // Relación: un usuario puede tener muchos reclamos
        public ICollection<Reclamo> Reclamos { get; set; } = new List<Reclamo>();

        // Relación: un usuario puede tener muchas respuestas (agregado para corregir el error)
        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}
