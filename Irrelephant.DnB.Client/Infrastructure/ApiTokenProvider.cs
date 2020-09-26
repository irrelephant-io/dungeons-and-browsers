using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Irrelephant.DnB.Client.Clients;
using Microsoft.JSInterop;

namespace Irrelephant.DnB.Client.Infrastructure
{
    public class ApiTokenProvider : IApiTokenProvider
    {
        private IDictionary<string, string> TokenCache { get; } = new Dictionary<string, string>();

        private readonly IJSRuntime _jsRuntime;

        private readonly IAuthHttpClient _authClient;

        public ApiTokenProvider(IJSRuntime jsRuntime, IAuthHttpClient authClient)
        {
            _jsRuntime = jsRuntime;
            _authClient = authClient;
        }

        public async Task<string> GetToken()
        {
            var existingIdToken = await _jsRuntime.InvokeAsync<string>("getCurrentIdToken");
            if (!string.IsNullOrEmpty(existingIdToken)) {
                if (!TokenCache.ContainsKey(existingIdToken))
                {
                    var acquiredToken = await _authClient.LogInAsync(existingIdToken);
                    TokenCache[existingIdToken] = acquiredToken;
                }
                return TokenCache[existingIdToken];
            }

            throw new AuthenticationException("Id token not found. Are we trying to authorize before authentication?");
        }
    }
}
