using FluentValidation;
using SupportApi.Models;

namespace SupportApi.Validators
{
    public class ReclamoValidator : AbstractValidator<Reclamo>
    {
        public ReclamoValidator()
        {
            RuleFor(r => r.Titulo).NotEmpty();
            RuleFor(r => r.Descripcion).NotEmpty();
            RuleFor(r => r.UsuarioId).NotEmpty();
        }
    }
}
