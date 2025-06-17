using FluentValidation;

namespace EncuestaAPI.Models.Validators
{
    public class PreguntaValidator : AbstractValidator<Pregunta>
    {
        public PreguntaValidator() 
        {
            RuleFor(x => x.Texto).NotEmpty().WithMessage("La pregunta no puede estar vacía.")
            .MaximumLength(5000).WithMessage("La pregunta no puede exceder los 5000 caracteres.");
        }
    }
}
