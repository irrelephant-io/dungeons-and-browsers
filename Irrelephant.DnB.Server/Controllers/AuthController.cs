using System.Threading.Tasks;
using Irrelephant.DnB.Server.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Irrelephant.DnB.Server.Controllers
{
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] string idToken)
        {
            var token = await _authService.Authenticate(idToken);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(token);
        }
    }
}
