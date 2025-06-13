using FluentValidation;

namespace EncuestaAPI.Models.Validators
{
    public class RespuestasValidator : AbstractValidator<Respuestas>
    {


        public RespuestasValidator()
        {
            RuleFor(x => x.Respuestauno)
                .NotEmpty().WithMessage("La respuesta uno no puede estar vacía.")
                .MaximumLength(5000).WithMessage("La respuesta uno no puede exceder los 5000 caracteres.");
            RuleFor(x => x.Respuestados)
                .NotEmpty().WithMessage("La respuesta dos no puede estar vacía.")
                .MaximumLength(5000).WithMessage("La respuesta dos no puede exceder los 5000 caracteres.");
            RuleFor(x => x.Respuesta)
                .NotEmpty().WithMessage("La respuesta tres no puede estar vacía.")
                .MaximumLength(5000).WithMessage("La respuesta tres no puede exceder los 5000 caracteres.");
            RuleFor(x => x.Respuestatres)
                .NotEmpty().WithMessage("La respuesta cuatro no puede estar vacía.")
                .MaximumLength(5000).WithMessage("La respuesta cuatro no puede exceder los 5000 caracteres.");
            RuleFor(x => x.Respuestacuatro)
                .NotEmpty().WithMessage("La respuesta cinco no puede estar vacía.")
                .MaximumLength(5000).WithMessage("La respuesta cinco no puede exceder los 5000 caracteres.");
        }


    }
}
