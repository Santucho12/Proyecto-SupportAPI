using Microsoft.AspNetCore.SignalR;
using SupportApi.Hubs;

namespace SupportApi.Services
{
    public class RespuestaService : IRespuestaService
    {
        private readonly Repositories.IRespuestaRepository _repo;
        private readonly IHubContext<SoporteHub> _hubContext;
        private readonly Data.SupportDbContext _context;

        public RespuestaService(Repositories.IRespuestaRepository repo, IHubContext<SoporteHub> hubContext, Data.SupportDbContext context)
        {
            _repo = repo;
            _hubContext = hubContext;
            _context = context;
        }

        public async Task<List<Models.Respuesta>> ObtenerNoVistasAsync(Guid usuarioId)
        {
            return await _repo.ObtenerNoVistasAsync(usuarioId);
        }

        public async Task MarcarComoVistoAsync(Guid respuestaId)
        {
            await _repo.MarcarComoVistoAsync(respuestaId);
        }

        public async Task<Models.Respuesta> CrearRespuestaAsync(Models.Respuesta respuesta)
        {
            var nuevaRespuesta = await _repo.CrearRespuestaAsync(respuesta);
            // Notificar al usuario destinatario (UsuarioId del reclamo)
            if (nuevaRespuesta?.Reclamo != null)
            {
                var userId = nuevaRespuesta.Reclamo.UsuarioId.ToString();
                var mensaje = "Tienes una nueva respuesta a tu reclamo.";
                await _hubContext.Clients.User(userId).SendAsync("RecibirRespuesta", mensaje);

                // Obtener usuario desde el contexto
                var usuario = _context.Usuarios.Find(nuevaRespuesta.Reclamo.UsuarioId);
                if (usuario != null) {
                    // Notificar por email
                    try {
                        var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                        var sendGridClient = new SendGrid.SendGridClient(sendGridApiKey);
                        var from = new SendGrid.Helpers.Mail.EmailAddress("soporte@tusistema.com", "Soporte Reclamos");
                        var to = new SendGrid.Helpers.Mail.EmailAddress(usuario.CorreoElectronico);
                        var subject = "Tienes una nueva respuesta a tu reclamo";
                        var plainTextContent = $"Hola {usuario.Nombre}, tienes una nueva respuesta a tu reclamo.";
                        var htmlContent = $"<strong>Hola {usuario.Nombre}, tienes una nueva respuesta a tu reclamo.</strong>";
                        var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        await sendGridClient.SendEmailAsync(msg);
                    } catch {}
                    // Notificar por WhatsApp
                    try {
                        var twilioAccountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
                        var twilioAuthToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
                        Twilio.TwilioClient.Init(twilioAccountSid, twilioAuthToken);
                        var message = await Twilio.Rest.Api.V2010.Account.MessageResource.CreateAsync(
                            body: $"Hola {usuario.Nombre}, tienes una nueva respuesta a tu reclamo.",
                            from: new Twilio.Types.PhoneNumber("whatsapp:+14155238886"),
                            to: new Twilio.Types.PhoneNumber("whatsapp:+5492914405674")
                        );
                    } catch {}
                }
            }
            return nuevaRespuesta ?? respuesta;
        }
    }
}
