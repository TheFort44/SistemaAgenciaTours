using System;
using System.Collections.Generic;

namespace SistemaAgenciaTours.Models;

public partial class VistaTourConEstado
{
    public int TourId { get; set; }

    public string NombreTour { get; set; } = null!;

    public int PaisId { get; set; }

    public string NombrePais { get; set; } = null!;

    public int DestinoId { get; set; }

    public string NombreDestino { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public TimeOnly Hora { get; set; }

    public decimal Precio { get; set; }

    public decimal? Itbis { get; set; }

    public int DuracionHoras { get; set; }

    public DateTime? FechaHoraFin { get; set; }

    public string Estado { get; set; } = null!;
}
