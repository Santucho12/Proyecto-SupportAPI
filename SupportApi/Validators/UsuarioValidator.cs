using FluentValidation;
using SupportApi.Models;

namespace SupportApi.Validators
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(u => u.Nombre).NotEmpty();
            RuleFor(u => u.CorreoElectronico).NotEmpty().EmailAddress();
            RuleFor(u => u.HashContrasena).NotEmpty();
        }
    }
}
