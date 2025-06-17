using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Aplicacionencuesta
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdEncuesta { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public virtual Encuesta IdEncuestaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Respuesta> Respuesta { get; set; } = new List<Respuesta>();
}
