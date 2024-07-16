using Microsoft.AspNetCore.Mvc;

namespace Automotive.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
