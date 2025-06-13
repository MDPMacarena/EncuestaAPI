using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Pregunta
{
    public int Id { get; set; }

    public string Pregunta1 { get; set; } = null!;

    public int IdRespuesta { get; set; }

    public virtual Respuestas IdRespuestaNavigation { get; set; } = null!;

    public virtual ICollection<ListaPregunta> ListaPregunta { get; set; } = new List<ListaPregunta>();
}
