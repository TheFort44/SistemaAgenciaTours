using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SistemaAgenciaTours.Helpers;
using SistemaAgenciaTours.Models;
using System.Collections.Generic;
using System.Text;

namespace SistemaAgenciaTours.Controllers
{
    public class ToursController : Controller
    {
        static string cadena = "Data Source=DESKTOP-GBKGIUE\\SQLEXPRESS;Initial Catalog=SistemaAgenciaTours;Integrated Security=true;TrustServerCertificate=True;";

        private List<ToursViewModel> ObtenerTours()
        {
            var tours = new List<ToursViewModel>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                string sql = "SELECT * FROM Vista_TourConEstado";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        tours.Add(new ToursViewModel
                        {
                            TourID = dr.GetInt32(dr.GetOrdinal("TourID")),
                            NombreTour = dr.GetString(dr.GetOrdinal("NombreTour")),
                            PaisID = dr.GetInt32(dr.GetOrdinal("PaisID")),
                            NombrePais = dr.GetString(dr.GetOrdinal("NombrePais")),
                            DestinoID = dr.GetInt32(dr.GetOrdinal("DestinoID")),
                            NombreDestino = dr.GetString(dr.GetOrdinal("NombreDestino")),
                            Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha")),
                            Hora = dr.GetTimeSpan(dr.GetOrdinal("Hora")),
                            Precio = dr.GetDecimal(dr.GetOrdinal("Precio")),
                            ITBIS = dr.GetDecimal(dr.GetOrdinal("ITBIS")),
                            Estado = dr.GetString(dr.GetOrdinal("Estado")),
                            DuracionDias = dr.GetInt32(dr.GetOrdinal("DuracionDias")),
                            FechaHoraFin = dr.GetDateTime(dr.GetOrdinal("FechaHoraFin")),
                            ImagenRuta = dr.IsDBNull(dr.GetOrdinal("ImagenRuta")) ? null : dr.GetString(dr.GetOrdinal("ImagenRuta"))
                        });
                    }
                }
            }
            return tours;
        }

        public IActionResult Index()
        {
            var tours = ObtenerTours();
            return View(tours);
        }

        public IActionResult IndexAdministrador()
        {
            var tours = ObtenerTours();
            return View("IndexAdministrador", tours);
        }

        private List<ToursViewModel> ObtenerPaises()
        {
            var paises = new List<ToursViewModel>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                string sql = "SELECT DISTINCT PaisID, NombrePais FROM Pais ORDER BY NombrePais";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        paises.Add(new ToursViewModel
                        {
                            PaisID = dr.GetInt32(dr.GetOrdinal("PaisID")),
                            NombrePais = dr.GetString(dr.GetOrdinal("NombrePais"))
                        });
                    }
                }
            }
            return paises;
        }

        private List<ToursViewModel> ObtenerDestinos()
        {
            var destinos = new List<ToursViewModel>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                string sql = "SELECT DISTINCT DestinoID, NombreDestino FROM Destino ORDER BY NombreDestino";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        destinos.Add(new ToursViewModel
                        {
                            DestinoID = dr.GetInt32(dr.GetOrdinal("DestinoID")),
                            NombreDestino = dr.GetString(dr.GetOrdinal("NombreDestino"))
                        });
                    }
                }
            }
            return destinos;
        }

        public IActionResult Crear()


        {
            ViewBag.Paises = ObtenerPaises()
    .Select(p => new SelectListItem { Value = p.PaisID.ToString(), Text = p.NombrePais })
    .ToList();

            ViewBag.Destinos = ObtenerDestinos()
                .Select(d => new SelectListItem { Value = d.DestinoID.ToString(), Text = d.NombreDestino })
                .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ToursViewModel tour, IFormFile Imagen)
        {
            string rutaImagen = null;

            if (Imagen != null && Imagen.Length > 0)
            {
                var nombreArchivo = Path.GetFileName(Imagen.FileName);
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes");

                if (!Directory.Exists(rutaCarpeta))
                    Directory.CreateDirectory(rutaCarpeta);

                var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await Imagen.CopyToAsync(stream);
                }

                rutaImagen = "/imagenes/" + nombreArchivo;
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                string sql = @"INSERT INTO Tour 
                       (NombreTour, PaisID, DestinoID, Fecha, Hora, Precio, ImagenRuta) 
                       VALUES 
                       (@NombreTour, @PaisID, @DestinoID, @Fecha, @Hora, @Precio, @ImagenRuta)";

                SqlCommand cmd = new SqlCommand(sql, cn);

                cmd.Parameters.AddWithValue("@NombreTour", tour.NombreTour);
                cmd.Parameters.AddWithValue("@PaisID", tour.PaisID);
                cmd.Parameters.AddWithValue("@DestinoID", tour.DestinoID);
                cmd.Parameters.AddWithValue("@Fecha", tour.Fecha);
                cmd.Parameters.AddWithValue("@Hora", tour.Hora);
                cmd.Parameters.AddWithValue("@Precio", tour.Precio);
                cmd.Parameters.AddWithValue("@ImagenRuta", rutaImagen ?? (object)DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("IndexAdministrador");
        }

        public IActionResult Editar(int id)
        {
            var tour = ObtenerTours().Find(t => t.TourID == id);
            if (tour == null) return NotFound();

            ViewBag.Paises = ObtenerPaises()
                .Select(p => new SelectListItem
                {
                    Value = p.PaisID.ToString(),
                    Text = p.NombrePais,
                    Selected = (p.PaisID == tour.PaisID)
                })
                .ToList();

            ViewBag.Destinos = ObtenerDestinos()
                .Select(d => new SelectListItem
                {
                    Value = d.DestinoID.ToString(),
                    Text = d.NombreDestino,
                    Selected = (d.DestinoID == tour.DestinoID)
                })
                .ToList();

            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ToursViewModel tour, IFormFile Imagen)
        {

            ViewBag.Paises = ObtenerPaises().Select(p => new SelectListItem
            {
                Value = p.PaisID.ToString(),
                Text = p.NombrePais,
                Selected = (p.PaisID == tour.PaisID)
            }).ToList();

            ViewBag.Destinos = ObtenerDestinos().Select(d => new SelectListItem
            {
                Value = d.DestinoID.ToString(),
                Text = d.NombreDestino,
                Selected = (d.DestinoID == tour.DestinoID)
            }).ToList();

           string rutaImagen = null;

            if (Imagen != null && Imagen.Length > 0)
            {
                var nombreArchivo = Path.GetFileName(Imagen.FileName);
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes");

                if (!Directory.Exists(rutaCarpeta))
                    Directory.CreateDirectory(rutaCarpeta);

                var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await Imagen.CopyToAsync(stream);
                }

                rutaImagen = "/imagenes/" + nombreArchivo;
            }
            else
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    string sqlImg = "SELECT ImagenRuta FROM Tour WHERE TourID = @id";
                    SqlCommand cmdImg = new SqlCommand(sqlImg, cn);
                    cmdImg.Parameters.AddWithValue("@id", tour.TourID);
                    cn.Open();
                    var result = cmdImg.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        rutaImagen = result.ToString();
                }
            }

            tour.ITBIS = tour.Precio * 0.18m;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                string sql = @"UPDATE Tour SET
                        NombreTour = @NombreTour,
                        PaisID = @PaisID,
                        DestinoID = @DestinoID,
                        Fecha = @Fecha,
                        Hora = @Hora,
                        Precio = @Precio,
                        ImagenRuta = @ImagenRuta
                       WHERE TourID = @TourID";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@TourID", tour.TourID);
                cmd.Parameters.AddWithValue("@NombreTour", tour.NombreTour);
                cmd.Parameters.AddWithValue("@PaisID", tour.PaisID);
                cmd.Parameters.AddWithValue("@DestinoID", tour.DestinoID);
                cmd.Parameters.AddWithValue("@Fecha", tour.Fecha);
                cmd.Parameters.AddWithValue("@Hora", tour.Hora);
                cmd.Parameters.AddWithValue("@Precio", tour.Precio);
                cmd.Parameters.AddWithValue("@ImagenRuta", (object) rutaImagen ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("IndexAdministrador");
        }


        public IActionResult Eliminar(int id)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                string sql = "DELETE FROM dbo.Tour WHERE TourID = @TourID";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@TourID", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("IndexAdministrador");
        }

        [HttpPost]
        public IActionResult AgregarAlCarrito(int id)
        {
            List<int> carrito = HttpContext.Session.GetObjectFromJson<List<int>>("Carrito") ?? new List<int>();

            if (!carrito.Contains(id))
            {
                carrito.Add(id);
                HttpContext.Session.SetObjectAsJson("Carrito", carrito);
            }

            return RedirectToAction("IndexAdministrador");
        }

        public IActionResult Carrito()
        {
            List<int> ids = HttpContext.Session.GetObjectFromJson<List<int>>("Carrito") ?? new List<int>();
            var tours = new List<ToursViewModel>();

            if (ids.Count > 0)
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    string sql = $"SELECT * FROM Vista_TourConEstado WHERE TourID IN ({string.Join(",", ids)})";
                    SqlCommand cmd = new SqlCommand(sql, cn);
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            tours.Add(new ToursViewModel
                            {
                                TourID = dr.GetInt32(dr.GetOrdinal("TourID")),
                                NombreTour = dr.GetString(dr.GetOrdinal("NombreTour")),
                                PaisID = dr.GetInt32(dr.GetOrdinal("PaisID")),
                                NombrePais = dr.GetString(dr.GetOrdinal("NombrePais")),
                                DestinoID = dr.GetInt32(dr.GetOrdinal("DestinoID")),
                                NombreDestino = dr.GetString(dr.GetOrdinal("NombreDestino")),
                                Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha")),
                                Hora = dr.GetTimeSpan(dr.GetOrdinal("Hora")),
                                Precio = dr.GetDecimal(dr.GetOrdinal("Precio")),
                                ITBIS = dr.GetDecimal(dr.GetOrdinal("ITBIS")),
                                Estado = dr.GetString(dr.GetOrdinal("Estado")),
                                ImagenRuta = dr.GetString(dr.GetOrdinal("ImagenRuta"))
                            });
                        }
                    }
                }
            }

            return View(tours);
        }

        [HttpPost]
        public IActionResult ExportarToursCSV()
        {
            var tours = ObtenerTours(); 

            var csv = new StringBuilder();
            csv.AppendLine("Tour,País,Destino,Fecha,Hora,Precio,ITBIS,Estado");

            foreach (var tour in tours)
            {
                csv.AppendLine($"\"{tour.NombreTour}\",\"{tour.NombrePais}\",\"{tour.NombreDestino}\"," +
                               $"\"{tour.Fecha:dd/MM/yyyy}\",\"{DateTime.Today.Add(tour.Hora):hh\\:mm}\"," +
                               $"\"{tour.Precio}\",\"{tour.ITBIS}\",\"{tour.Estado}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "ToursDisponibles.csv");
        }

        public IActionResult EliminarDelCarrito(int id)
        {
            List<int> carrito = HttpContext.Session.GetObjectFromJson<List<int>>("Carrito") ?? new List<int>();
            carrito.Remove(id);
            HttpContext.Session.SetObjectAsJson("Carrito", carrito);
            return RedirectToAction("Carrito");
        }

        public IActionResult PorDestino(int id)
        {
            var tours = new List<ToursViewModel>();

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                string sql = @"
            SELECT * FROM Vista_TourConEstado
            WHERE DestinoID = @DestinoID";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@DestinoID", id);
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        tours.Add(new ToursViewModel
                        {
                            TourID = dr.GetInt32(dr.GetOrdinal("TourID")),
                            NombreTour = dr.GetString(dr.GetOrdinal("NombreTour")),
                            PaisID = dr.GetInt32(dr.GetOrdinal("PaisID")),
                            NombrePais = dr.GetString(dr.GetOrdinal("NombrePais")),
                            DestinoID = dr.GetInt32(dr.GetOrdinal("DestinoID")),
                            NombreDestino = dr.GetString(dr.GetOrdinal("NombreDestino")),
                            Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha")),
                            Hora = dr.GetTimeSpan(dr.GetOrdinal("Hora")),
                            Precio = dr.GetDecimal(dr.GetOrdinal("Precio")),
                            ITBIS = dr.GetDecimal(dr.GetOrdinal("ITBIS")),
                            Estado = dr.GetString(dr.GetOrdinal("Estado")),
                            DuracionDias = dr.GetInt32(dr.GetOrdinal("DuracionDias")),
                            ImagenRuta = dr.IsDBNull(dr.GetOrdinal("ImagenRuta")) ? null : dr.GetString(dr.GetOrdinal("ImagenRuta"))
                        });
                    }
                }
            }

            return View(tours);
        }

    }
}
