using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Respuestas
{
    public int Id { get; set; }

    public string Respuestauno { get; set; } = null!;

    public string Respuestados { get; set; } = null!;

    public string Respuesta { get; set; } = null!;

    public string Respuestatres { get; set; } = null!;

    public string Respuestacuatro { get; set; } = null!;

    public virtual ICollection<Pregunta> Pregunta { get; set; } = new List<Pregunta>();
}
