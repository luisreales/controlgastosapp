using Microsoft.AspNetCore.Mvc;

namespace ControlGastosApp.Web.Controllers
{
    public class ErrorController : Controller
    {
        public new IActionResult NotFound()
        {
            return View();
        }
    }
} 