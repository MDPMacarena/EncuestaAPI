using FluentValidation;

namespace EncuestaAPI.Models.Validators
{
    public class RespuestaValidator : AbstractValidator<Detallerespuesta>
    {
        public RespuestaValidator()
        {
            RuleFor(x => x.IdRespuesta)
                .NotEmpty().WithMessage("Debe estar asociado a una respuesta principal")
                .GreaterThan(0).WithMessage("ID de respuesta inválido");
            RuleFor(x => x.Valor)
               .NotEmpty().WithMessage("La calificación es requerida")
               .InclusiveBetween(1, 5).WithMessage("La calificación debe estar entre 1 y 5");
            RuleFor(x => x.Valor)
                .InclusiveBetween(1, 5)
                .WithMessage("La calificación debe estar entre 1 y 5");
            RuleFor(x => x.IdPregunta)
             .GreaterThan(0)
             .WithMessage("Debe seleccionar una pregunta válida");
        }
    }
}
