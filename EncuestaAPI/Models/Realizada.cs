using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class Realizada
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public byte[] Confirmar { get; set; } = null!;

    public virtual ICollection<ListaEncuesta> ListaEncuesta { get; set; } = new List<ListaEncuesta>();
}
