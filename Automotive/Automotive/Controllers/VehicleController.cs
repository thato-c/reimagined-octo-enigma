using Microsoft.AspNetCore.Mvc;

namespace Automotive.Controllers
{
    public class VehicleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
