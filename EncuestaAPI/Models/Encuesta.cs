using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Encuesta
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdUsuario { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Aplicacionencuesta> Aplicacionencuesta { get; set; } = new List<Aplicacionencuesta>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pregunta> Pregunta { get; set; } = new List<Pregunta>();
}
