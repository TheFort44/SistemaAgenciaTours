using System;
using System.Collections.Generic;

namespace SistemaAgenciaTours.Models;

public partial class Tour
{
    public int TourId { get; set; }

    public string NombreTour { get; set; } = null!;

    public int PaisId { get; set; }

    public int DestinoId { get; set; }

    public DateOnly Fecha { get; set; }

    public TimeOnly Hora { get; set; }

    public decimal Precio { get; set; }

    public decimal? Itbis { get; set; }

    public virtual Destino Destino { get; set; } = null!;

    public virtual Pai Pais { get; set; } = null!;
}
