using System.Threading.Tasks;
using Irrelephant.DnB.Server.Authentication.Models;
using Irrelephant.DnB.Server.Authentication.Services;

namespace Irrelephant.DnB.Server.Tests.Infrastructure
{
    internal class TestTokenValidator : ITokenValidator
    {
        public Task<IdTokenValidationResult> ValidateIdToken(string idToken)
        {
            return Task.FromResult(new IdTokenValidationResult {
                DisplayName = "Tester", Email = "dungeons-and-browsers-tester@gmail.com", IsValid = true
            });
        }
    }
}
