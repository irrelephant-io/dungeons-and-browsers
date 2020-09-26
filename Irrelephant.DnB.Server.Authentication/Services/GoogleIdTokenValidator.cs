using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Irrelephant.DnB.Server.Authentication.Models;
using Irrelephant.DnB.Server.Authentication.Options;
using Microsoft.Extensions.Options;

namespace Irrelephant.DnB.Server.Authentication.Services
{
    public class GoogleIdTokenValidator : ITokenValidator
    {
        private readonly IOptionsMonitor<GoogleApiCredentials> _credentials;

        public GoogleIdTokenValidator(IOptionsMonitor<GoogleApiCredentials> credentials)
        {
            _credentials = credentials;
        }

        private bool ValidationResultValid(GoogleJsonWebSignature.Payload result)
        {
            if (result != null && result.Audience != null && result.EmailVerified)
            {
                if (result.Audience is IList<string> audiencesList)
                {
                    return audiencesList.Any(audience => audience.Contains(_credentials.CurrentValue.ClientId));
                }

                if (result.Audience is string singleAudience)
                {
                    return singleAudience.Contains(_credentials.CurrentValue.ClientId);
                }
            }

            return false;
        }

        public async Task<IdTokenValidationResult> ValidateIdToken(string idToken)
        {
            try
            {
                var result = await GoogleJsonWebSignature.ValidateAsync(idToken);
                return new IdTokenValidationResult {
                    IsValid = ValidationResultValid(result),
                    Email = result?.Email,
                    DisplayName = result?.GivenName
                };
            }
            catch (InvalidJwtException)
            {
                return new IdTokenValidationResult {IsValid = false};
            }
        }
    }
}
