namespace SupportApi.DTOs
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public SupportApi.Models.RolUsuario Rol { get; set; } = SupportApi.Models.RolUsuario.Usuario;
    }
}
