using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irrelephant.DnB.Server.Controllers
{
    public class HealthController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }

        [HttpGet]
        [Route("auth")]
        public IActionResult TestAuth()
        {
            return Ok("All green!");
        }
    }
}
