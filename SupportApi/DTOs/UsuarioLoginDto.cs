namespace SupportApi.DTOs
{
    /// <summary>Credenciales para login de usuario.</summary>
    /// <example>
    /// {
    ///   "email": "usuario@example.com",
    ///   "password": "123456"
    /// }
    /// </example>
    public class UsuarioLoginDto
    {
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}
