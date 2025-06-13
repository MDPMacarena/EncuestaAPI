using FluentValidation;

namespace EncuestaAPI.Models.Validators
{
    public class ListaEncuestaValidator : AbstractValidator<ListaEncuesta>
    {
        public ListaEncuestaValidator() 
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
           
            RuleFor(x => x.FechaCreacion)
                .NotEmpty().WithMessage("La fecha de creación no puede estar vacía.");
        }
    }
}
