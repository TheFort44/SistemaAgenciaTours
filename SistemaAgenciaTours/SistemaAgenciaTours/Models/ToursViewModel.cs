namespace SistemaAgenciaTours.Models
{
    public class ToursViewModel
    {
        public int TourID { get; set; }
        public string NombreTour { get; set; }
        public int PaisID { get; set; }
        public string NombrePais { get; set; }
        public int DestinoID { get; set; }
        public string NombreDestino { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public decimal Precio { get; set; }
        public decimal ITBIS { get; set; }
        public string Estado { get; set; }
    }
}
