using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaAgenciaTours.Models;

namespace SistemaAgenciaTours.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
