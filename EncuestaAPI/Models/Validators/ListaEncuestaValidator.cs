using FluentValidation;

namespace EncuestaAPI.Models.Validators
{
    public class ListaEncuestaValidator : AbstractValidator<Encuesta>
    {
        public ListaEncuestaValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}
