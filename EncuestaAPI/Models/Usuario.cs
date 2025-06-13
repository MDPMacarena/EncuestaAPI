using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string NumControl { get; set; } = null!;

    public string Contrsena { get; set; } = null!;

    public virtual ICollection<ListaEncuesta> ListaEncuesta { get; set; } = new List<ListaEncuesta>();

    public virtual ICollection<ListaEncuestados> ListaEncuestados { get; set; } = new List<ListaEncuestados>();
}
