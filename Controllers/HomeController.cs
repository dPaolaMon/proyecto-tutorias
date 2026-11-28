using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tutorias.Models;

namespace tutorias.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Catalogo");
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Estado(int id)
    {
        var (titulo, mensaje) = id switch
        {
            404 => ("Página no encontrada", "La dirección que buscas no existe o el registro ya no está disponible."),
            400 => ("Solicitud no válida", "No pudimos procesar la solicitud. Vuelve a cargar la página e intenta de nuevo."),
            _ => ("Algo salió mal", "Ocurrió un error al procesar tu solicitud. Intenta de nuevo en unos minutos.")
        };

        return View("Error", new ErrorViewModel { Titulo = titulo, Mensaje = mensaje });
    }
}
