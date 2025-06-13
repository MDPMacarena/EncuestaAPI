using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class ListaEncuesta
{
    public int Id { get; set; }

    public int IdListaPregunta { get; set; }

    public int IdUsuario { get; set; }

    public byte[] Activo { get; set; } = null!;

    public int IdRealizada { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public virtual ListaPregunta IdListaPreguntaNavigation { get; set; } = null!;

    public virtual Realizada IdRealizadaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<ListaEncuestados> ListaEncuestados { get; set; } = new List<ListaEncuestados>();
}
