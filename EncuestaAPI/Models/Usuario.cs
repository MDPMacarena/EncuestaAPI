using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string NumControl { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public virtual ICollection<Aplicacionencuesta> Aplicacionencuesta { get; set; } = new List<Aplicacionencuesta>();

    public virtual ICollection<Encuesta> Encuesta { get; set; } = new List<Encuesta>();
}
