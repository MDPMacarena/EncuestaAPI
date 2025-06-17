using FluentValidation;

namespace EncuestaAPI.Models.Validators
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(usuario => usuario.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(45).WithMessage("El nombre no debe superar los 100 caracteres.");
            RuleFor(usuario => usuario.NumControl)
                .NotEmpty().WithMessage("El número de control es obligatorio.")
                .MinimumLength(8).WithMessage("El número de control debe tener al menos 8 caracteres.")
                .MaximumLength(8).WithMessage("El número de control debe tener exactamente 8 caracteres.");
            RuleFor(usuario => usuario.Contraseña)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no debe superar los 100 caracteres.")
                .Matches(@"^(?=.*[A-ZÑ])(?=.*[a-zñ]).*$")
                .WithMessage("La contraseña debe contener al menos una letra mayúscula, una letra minúscula y un dígito.");
        }
    }
}
