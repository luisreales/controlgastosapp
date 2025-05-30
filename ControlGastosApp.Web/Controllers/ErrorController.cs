using Microsoft.AspNetCore.Mvc;

namespace ControlGastosApp.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }
    }
} 