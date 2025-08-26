namespace SupportApi.DTOs
{
    /// <summary>DTO para crear usuario.</summary>
    /// <example>
    /// {
    ///   "nombre": "Juan Pérez",
    ///   "correoElectronico": "juan@ejemplo.com",
    ///   "password": "123456",
    ///   "rol": "Usuario"
    /// }
    /// </example>
    public class UsuarioCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = "Usuario";
    }
}
