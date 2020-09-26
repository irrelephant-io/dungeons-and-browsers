using System.Threading.Tasks;
using Irrelephant.DnB.Server.Authentication.Models;

namespace Irrelephant.DnB.Server.Authentication.Services
{
    public interface ITokenValidator
    {
        public Task<IdTokenValidationResult> ValidateIdToken(string idToken);
    }
}
