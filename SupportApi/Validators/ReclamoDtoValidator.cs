using FluentValidation;
using SupportApi.DTOs;

namespace SupportApi.Validators
{
    public class ReclamoDtoValidator : AbstractValidator<ReclamoDto>
    {
        public ReclamoDtoValidator()
        {
            RuleFor(r => r.Titulo).NotEmpty();
            RuleFor(r => r.Descripcion).NotEmpty();
            RuleFor(r => r.UsuarioId).NotEmpty();
        }
    }
}
