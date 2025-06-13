using System;
using System.Collections.Generic;

namespace EncuestaAPI.Models;

public partial class ListaEncuestados
{
    public int Id { get; set; }

    public int IdListaPreguntas { get; set; }

    public int IdUsuarios { get; set; }

    public int IdEncuestas { get; set; }

    public virtual ListaEncuesta IdEncuestasNavigation { get; set; } = null!;

    public virtual ListaPregunta IdListaPreguntasNavigation { get; set; } = null!;

    public virtual Usuario IdUsuariosNavigation { get; set; } = null!;
}
