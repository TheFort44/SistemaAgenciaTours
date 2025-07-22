using System;
using System.Collections.Generic;

namespace SistemaAgenciaTours.Models;

public partial class Pai
{
    public int PaisId { get; set; }

    public string NombrePais { get; set; } = null!;

    public virtual ICollection<Destino> Destinos { get; set; } = new List<Destino>();

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
