using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SistemaAgenciaTours.Models;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;

public class PaisesController : Controller
{
    private readonly string cadena = "Data Source=DESKTOP-GBKGIUE\\SQLEXPRESS;Initial Catalog=SistemaAgenciaTours;Integrated Security=true;TrustServerCertificate=True";
    private readonly IWebHostEnvironment _env;

    public PaisesController(IWebHostEnvironment env)
    {
        _env = env;
    }

    public IActionResult Index()
    {
        var paises = ObtenerPaisesConDestinos();
        return View(paises);
    }

    public IActionResult ExportarCSV()
    {
        var paisesResult = Index() as ViewResult;
        var paises = paisesResult?.Model as List<PaisViewModel>;

        var csv = new StringBuilder();
        csv.AppendLine("Pais,Destino");

        foreach (var pais in paises)
        {
            if (pais.Destinos.Count > 0)
            {
                foreach (var destino in pais.Destinos)
                {
                    csv.AppendLine($"{pais.NombrePais},{destino.Nombre}");
                }
            }
            else
            {
                csv.AppendLine($"{pais.NombrePais},");
            }
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", "PaisesConDestinos.csv");
    }

    public IActionResult ToursPorDestino(int id)
    {
        var tours = new List<ToursViewModel>();

        using (SqlConnection cn = new SqlConnection(cadena))
        {
            string sql = @"
                SELECT t.TourID, t.NombreTour, t.Fecha, t.Hora, t.Precio, t.Estado,
                       d.NombreDestino, p.NombrePais
                FROM dbo.Tour t
                INNER JOIN dbo.Destino d ON t.DestinoID = d.DestinoID
                INNER JOIN dbo.Pais p ON d.PaisID = p.PaisID
                WHERE d.DestinoID = @DestinoID";

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
                        Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha")),
                        Hora = dr.GetTimeSpan(dr.GetOrdinal("Hora")),
                        Precio = dr.GetDecimal(dr.GetOrdinal("Precio")),
                        Estado = dr.GetString(dr.GetOrdinal("Estado")),
                        NombreDestino = dr.GetString(dr.GetOrdinal("NombreDestino")),
                        NombrePais = dr.GetString(dr.GetOrdinal("NombrePais")),
                    });
                }
            }
        }

        return View("PorDestino", tours);
    }

 
    public IActionResult Admin()
    {
        var paises = ObtenerPaises();
        return View(paises);
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Crear(PaisViewModel model)
    {
       
        string rutaArchivo = "/img/banderas/default.png";

        if (model.BanderaFile != null && model.BanderaFile.Length > 0)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "img", "banderas");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BanderaFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.BanderaFile.CopyTo(fileStream);
            }

            rutaArchivo = "/img/banderas/" + uniqueFileName;
        }

        using (SqlConnection cn = new SqlConnection(cadena))
        {
            string sql = "INSERT INTO dbo.Pais (NombrePais, BanderaRuta) VALUES (@NombrePais, @BanderaRuta)";
            SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@NombrePais", model.NombrePais);
            cmd.Parameters.AddWithValue("@BanderaRuta", rutaArchivo);
            cn.Open();
            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Admin");
    }

    public IActionResult Editar(int id)
    {
        var pais = ObtenerPaisPorId(id);
        if (pais == null)
            return NotFound();

        return View(pais);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(PaisViewModel model)
    {
        string rutaArchivo = null;

        if (model.BanderaFile != null && model.BanderaFile.Length > 0)
        {
            var nombreArchivo = Path.GetFileName(model.BanderaFile.FileName);
            var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/banderas");

            if (!Directory.Exists(rutaCarpeta))
                Directory.CreateDirectory(rutaCarpeta);

            var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await model.BanderaFile.CopyToAsync(stream);
            }

            rutaArchivo = "/img/banderas/" + nombreArchivo;
        }
        else
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                string sqlImg = "SELECT BanderaRuta FROM dbo.Pais WHERE PaisID = @id";
                SqlCommand cmdImg = new SqlCommand(sqlImg, cn);
                cmdImg.Parameters.AddWithValue("@id", model.PaisID);
                cn.Open();
                var result = cmdImg.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    rutaArchivo = result.ToString();
                else
                    rutaArchivo = "/img/banderas/default.png";
            }
        }

        using (SqlConnection cn = new SqlConnection(cadena))
        {
            string sql = @"UPDATE dbo.Pais SET
                        NombrePais = @NombrePais,
                        BanderaRuta = @BanderaRuta
                       WHERE PaisID = @PaisID";

            SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@NombrePais", model.NombrePais);
            cmd.Parameters.AddWithValue("@BanderaRuta", rutaArchivo);
            cmd.Parameters.AddWithValue("@PaisID", model.PaisID);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Admin");
    }

    [HttpPost]
    public IActionResult Eliminar(int id)
    {
        using (SqlConnection cn = new SqlConnection(cadena))
        {
            string sql = "DELETE FROM dbo.Pais WHERE PaisID = @PaisID";
            SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@PaisID", id);
            cn.Open();
            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Admin");
    }


    private List<PaisViewModel> ObtenerPaisesConDestinos()
    {
        var paises = new List<PaisViewModel>();

        using (SqlConnection cn = new SqlConnection(cadena))
        {
            string sql = @"
                SELECT p.PaisID, p.NombrePais, p.BanderaRuta, d.DestinoID, d.NombreDestino
                FROM dbo.Pais p
                LEFT JOIN dbo.Destino d ON p.PaisID = d.PaisID
                ORDER BY p.NombrePais, d.NombreDestino";

            SqlCommand cmd = new SqlCommand(sql, cn);
            cn.Open();

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    int paisId = dr.GetInt32(dr.GetOrdinal("PaisID"));
                    var pais = paises.Find(p => p.PaisID == paisId);

                    if (pais == null)
                    {
                        pais = new PaisViewModel
                        {
                            PaisID = paisId,
                            NombrePais = dr.GetString(dr.GetOrdinal("NombrePais")),
                            BanderaRuta = dr.IsDBNull(dr.GetOrdinal("BanderaRuta")) ? "/img/banderas/default.png" : dr.GetString(dr.GetOrdinal("BanderaRuta")),
                            Destinos = new List<DestinoViewModel>()
                        };
                        paises.Add(pais);
                    }

                    if (!dr.IsDBNull(dr.GetOrdinal("DestinoID")))
                    {
                        pais.Destinos.Add(new DestinoViewModel
                        {
                            DestinoID = dr.GetInt32(dr.GetOrdinal("DestinoID")),
                            Nombre = dr.GetString(dr.GetOrdinal("NombreDestino")),
                            PaisID = paisId
                        });
                    }
                }
            }
        }

        return paises;
    }

    private List<PaisViewModel> ObtenerPaises()
    {
        var paises = new List<PaisViewModel>();

        using (SqlConnection cn = new SqlConnection(cadena))
        {
            string sql = "SELECT PaisID, NombrePais, BanderaRuta FROM dbo.Pais ORDER BY NombrePais";
            SqlCommand cmd = new SqlCommand(sql, cn);
            cn.Open();

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    paises.Add(new PaisViewModel
                    {
                        PaisID = dr.GetInt32(dr.GetOrdinal("PaisID")),
                        NombrePais = dr.GetString(dr.GetOrdinal("NombrePais")),
                        BanderaRuta = dr.IsDBNull(dr.GetOrdinal("BanderaRuta")) ? "/img/banderas/default.png" : dr.GetString(dr.GetOrdinal("BanderaRuta"))
                    });
                }
            }
        }

        return paises;
    }

    private PaisViewModel ObtenerPaisPorId(int id)
    {
        using (SqlConnection cn = new SqlConnection(cadena))
        {
            string sql = "SELECT PaisID, NombrePais, BanderaRuta FROM dbo.Pais WHERE PaisID = @PaisID";
            SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@PaisID", id);
            cn.Open();

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    return new PaisViewModel
                    {
                        PaisID = dr.GetInt32(dr.GetOrdinal("PaisID")),
                        NombrePais = dr.GetString(dr.GetOrdinal("NombrePais")),
                        BanderaRuta = dr.IsDBNull(dr.GetOrdinal("BanderaRuta")) ? "/img/banderas/default.png" : dr.GetString(dr.GetOrdinal("BanderaRuta"))
                    };
                }
            }
        }
        return null;
    }
}
