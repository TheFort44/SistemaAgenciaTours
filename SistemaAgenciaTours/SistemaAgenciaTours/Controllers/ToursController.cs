using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaAgenciaTours.Models;
using SistemaAgenciaTours.Helpers;
using System.Collections.Generic;

namespace SistemaAgenciaTours.Controllers
{
    public class ToursController : Controller
    {
        static string cadena = "Data Source=DESKTOP-GBKGIUE\\SQLEXPRESS;Initial Catalog=SistemaAgenciaTours;Integrated Security=true;TrustServerCertificate=True;";

        // Obtener lista de tours
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
                            Estado = dr.GetString(dr.GetOrdinal("Estado"))
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

        // GET: Tours/Crear
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

        // POST: Tours/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ToursViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    string sql = @"INSERT INTO Tours 
                        (NombreTour, PaisID, DestinoID, Fecha, Hora, Precio, ITBIS, Estado) 
                        VALUES (@NombreTour, @PaisID, @DestinoID, @Fecha, @Hora, @Precio, @ITBIS, @Estado)";
                    SqlCommand cmd = new SqlCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@NombreTour", model.NombreTour);
                    cmd.Parameters.AddWithValue("@PaisID", model.PaisID);
                    cmd.Parameters.AddWithValue("@DestinoID", model.DestinoID);
                    cmd.Parameters.AddWithValue("@Fecha", model.Fecha);
                    cmd.Parameters.AddWithValue("@Hora", model.Hora);
                    cmd.Parameters.AddWithValue("@Precio", model.Precio);
                    cmd.Parameters.AddWithValue("@ITBIS", model.ITBIS);
                    cmd.Parameters.AddWithValue("@Estado", model.Estado);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(IndexAdministrador));
            }
            catch
            {
                ModelState.AddModelError("", "Error al guardar el tour.");
                return View(model);
            }
        }

        // GET: Tours/Editar/5
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

        // POST: Tours/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, ToursViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    string sql = @"UPDATE Tours SET 
                        NombreTour=@NombreTour, PaisID=@PaisID, DestinoID=@DestinoID, 
                        Fecha=@Fecha, Hora=@Hora, Precio=@Precio, ITBIS=@ITBIS, Estado=@Estado 
                        WHERE TourID=@id";
                    SqlCommand cmd = new SqlCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@NombreTour", model.NombreTour);
                    cmd.Parameters.AddWithValue("@PaisID", model.PaisID);
                    cmd.Parameters.AddWithValue("@DestinoID", model.DestinoID);
                    cmd.Parameters.AddWithValue("@Fecha", model.Fecha);
                    cmd.Parameters.AddWithValue("@Hora", model.Hora);
                    cmd.Parameters.AddWithValue("@Precio", model.Precio);
                    cmd.Parameters.AddWithValue("@ITBIS", model.ITBIS);
                    cmd.Parameters.AddWithValue("@Estado", model.Estado);
                    cmd.Parameters.AddWithValue("@id", id);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(IndexAdministrador));
            }
            catch
            {
                ModelState.AddModelError("", "Error al actualizar el tour.");
                return View(model);
            }
        }

        // GET: Tours/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var tour = ObtenerTours().Find(t => t.TourID == id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        // POST: Tours/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    string sql = "DELETE FROM Tours WHERE TourID = @id";
                    SqlCommand cmd = new SqlCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(IndexAdministrador));
            }
            catch
            {
                return RedirectToAction(nameof(Eliminar), new { id });
            }
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
                                Estado = dr.GetString(dr.GetOrdinal("Estado"))
                            });
                        }
                    }
                }
            }

            return View(tours);
        }

        public IActionResult EliminarDelCarrito(int id)
        {
            List<int> carrito = HttpContext.Session.GetObjectFromJson<List<int>>("Carrito") ?? new List<int>();
            carrito.Remove(id);
            HttpContext.Session.SetObjectAsJson("Carrito", carrito);
            return RedirectToAction("Carrito");
        }
    }
}
