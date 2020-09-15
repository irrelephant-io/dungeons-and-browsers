using Microsoft.AspNetCore.Mvc;

namespace Irrelephant.DnB.Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Ok("This is FIIINE");
        }
    }
}
