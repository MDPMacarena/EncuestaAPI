using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Detallerespuesta
{
    public int Id { get; set; }

    public int IdRespuesta { get; set; }

    public int IdPregunta { get; set; }

    public int Valor { get; set; }

    public virtual Pregunta IdPreguntaNavigation { get; set; } = null!;

    public virtual Respuesta IdRespuestaNavigation { get; set; } = null!;
}
