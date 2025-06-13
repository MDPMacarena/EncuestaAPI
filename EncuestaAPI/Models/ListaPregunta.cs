using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class ListaPregunta
{
    public int Id { get; set; }

    public int IdPregunta { get; set; }

    public virtual Pregunta IdPreguntaNavigation { get; set; } = null!;

    public virtual ICollection<ListaEncuesta> ListaEncuesta { get; set; } = new List<ListaEncuesta>();

    public virtual ICollection<ListaEncuestados> ListaEncuestados { get; set; } = new List<ListaEncuestados>();
}
