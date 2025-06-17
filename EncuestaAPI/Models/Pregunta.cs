using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Pregunta
{
    public int Id { get; set; }

    public string Texto { get; set; } = null!;

    public int IdEncuesta { get; set; }

    public int Orden { get; set; }

    public virtual ICollection<Detallerespuesta> Detallerespuesta { get; set; } = new List<Detallerespuesta>();

    public virtual Encuesta IdEncuestaNavigation { get; set; } = null!;
}
