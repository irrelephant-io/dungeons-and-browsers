using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irrelephant.DnB.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/[controller]")]
    public abstract class ApiControllerBase : Controller
    {
    }
}
