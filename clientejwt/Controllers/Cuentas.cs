using Microsoft.AspNetCore.Mvc;

namespace clientejwt.Controllers
{
    public class Cuentas : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
