using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Threading.Tasks;

namespace SupportApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpPost("ActualizarNotificacion")]
        public async Task<IActionResult> ActualizarNotificacion([FromBody] NotificacionDto dto)
        {
            // Aquí guardarías la preferencia en la base de datos
            // Simulación:
            bool guardado = true;
            if (guardado)
                return Ok(new { success = true });
            else
                return BadRequest(new { success = false, message = "No se pudo guardar la preferencia" });
        }

        [HttpPost("EnviarNotificacionTest")]
        public async Task<IActionResult> EnviarNotificacionTest([FromBody] NotificacionTestDto dto)
        {
            // EMAIL (SendGrid API Key desde variable de entorno)
            var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var sendGridClient = new SendGridClient(sendGridApiKey);
            var from = new EmailAddress("soporte@tusistema.com", "Soporte Reclamos");
            var to = new EmailAddress(dto.Email);
            var subject = "Notificación de prueba";
            var plainTextContent = "Este es un email de prueba.";
            var htmlContent = "<strong>Este es un email de prueba.</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await sendGridClient.SendEmailAsync(msg);

        }
            // EMAIL (SendGrid API Key desde variable de entorno)
            var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var sendGridClient = new SendGridClient(sendGridApiKey);
            var from = new EmailAddress("soporte@tusistema.com", "Soporte Reclamos");
            var to = new EmailAddress(dto.Email);
            var subject = "Notificación de prueba";
            var plainTextContent = "Este es un email de prueba.";
            var htmlContent = "<strong>Este es un email de prueba.</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await sendGridClient.SendEmailAsync(msg);

            // SMS
    public class NotificacionDto
    {
        public string Tipo { get; set; }
        public bool Estado { get; set; }
    }

    public class NotificacionTestDto
    {
        public string Email { get; set; }
        public string Celular { get; set; }
    }
}
