using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Respuesta
{
    public int Id { get; set; }

    public int IdAplicacion { get; set; }

    public string NumControlAlumno { get; set; } = null!;

    public string NombreAlumno { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public virtual ICollection<Detallerespuesta> Detallerespuesta { get; set; } = new List<Detallerespuesta>();

    public virtual Aplicacionencuesta IdAplicacionNavigation { get; set; } = null!;
}
