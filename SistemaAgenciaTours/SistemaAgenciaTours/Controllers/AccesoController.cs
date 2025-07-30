using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SistemaAgenciaTours.Models;
using System.Data;

namespace SistemaAgenciaTours.Controllers
{
    public class AccesoController : Controller
    {
        static string cadena = "Data Source=DESKTOP-GBKGIUE\\SQLEXPRESS;Initial Catalog=DB_ACCESO;Integrated Security=true;TrustServerCertificate=True";

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Usuario oUsuario)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                object result = cmd.ExecuteScalar();
                oUsuario.IdUsuario = result != null ? Convert.ToInt32(result) : 0;
            }

            if (oUsuario.IdUsuario != 0)
            {
                HttpContext.Session.SetString("usuario", JsonConvert.SerializeObject(oUsuario));
                return RedirectToAction("IndexAdministrador", "Tours");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Usuario oUsuario)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido (por ejemplo, contraseñas no coinciden), se devuelve la vista con errores.
                return View(oUsuario);
            }

            bool registrado;
            string mensaje;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();
            }

            if (registrado)
            {
                TempData["mensaje"] = mensaje;
                return RedirectToAction("Login"); // o a donde tú quieras
            }
            else
            {
                ViewData["mensaje"] = mensaje;
                return View(oUsuario);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Acceso");
        }

        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Acceso");
        }
    }
}
