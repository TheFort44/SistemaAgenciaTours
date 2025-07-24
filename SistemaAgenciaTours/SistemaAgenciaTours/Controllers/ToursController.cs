using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SistemaAgenciaTours.Models;

namespace SistemaAgenciaTours.Controllers
{
    public class ToursController : Controller
    {
        static string cadena = "Data Source=DESKTOP-GBKGIUE\\SQLEXPRESS;Initial Catalog=SistemaAgenciaTours;Integrated Security=true;TrustServerCertificate=True;";

        public IActionResult Index()
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

            return View(tours);
        }

        // GET: ToursController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ToursController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ToursController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ToursController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ToursController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ToursController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ToursController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
