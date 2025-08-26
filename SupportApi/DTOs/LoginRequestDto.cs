namespace SupportApi.DTOs
{
    public class LoginRequestDto
    {
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}
