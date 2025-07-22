using System;
using System.Collections.Generic;

namespace SistemaAgenciaTours.Models;

public partial class Destino
{
    public int DestinoId { get; set; }

    public int PaisId { get; set; }

    public string NombreDestino { get; set; } = null!;

    public int DuracionHoras { get; set; }

    public virtual Pai Pais { get; set; } = null!;

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
