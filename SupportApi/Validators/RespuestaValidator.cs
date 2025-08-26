using FluentValidation;
using SupportApi.Models;

namespace SupportApi.Validators
{
    public class RespuestaValidator : AbstractValidator<Respuesta>
    {
        public RespuestaValidator()
        {
            RuleFor(r => r.Contenido).NotEmpty();
            RuleFor(r => r.UsuarioId).NotEmpty();
            RuleFor(r => r.ReclamoId).NotEmpty();
        }
    }
}
