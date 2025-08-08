namespace SistemaAgenciaTours.Models
{
    public class PaisViewModel
    {
        public int PaisID { get; set; }
        public string NombrePais { get; set; }
        public string BanderaRuta { get; set; }

        public IFormFile BanderaFile { get; set; }

        public List<DestinoViewModel> Destinos { get; set; } = new List<DestinoViewModel>();
    }
}