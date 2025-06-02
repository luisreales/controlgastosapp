using Microsoft.AspNetCore.Mvc;

namespace ControlGastosApp.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DatabaseError()
        {
            return View();
        }
    }
}
