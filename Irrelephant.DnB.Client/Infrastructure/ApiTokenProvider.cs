using Irrelephant.DnB.DataTransfer.Clients;
using Irrelephant.DnB.DataTransfer.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Irrelephant.DnB.Client.Infrastructure;

public class ApiTokenProvider : IApiTokenProvider
{
    private IDictionary<string, string> TokenCache { get; } = new Dictionary<string, string>();
    private readonly IAuthHttpClient _authClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly IOptions<OidcProviderOptions> _providerOptions;
    private readonly ILogger<ApiTokenProvider> _logger; 

    public ApiTokenProvider(
        IAuthHttpClient authClient,
        IJSRuntime jsRuntime,
        IOptions<OidcProviderOptions> providerOptions,
        ILogger<ApiTokenProvider> logger)
    {
        _authClient = authClient;
        _jsRuntime = jsRuntime;
        _providerOptions = providerOptions;
        _logger = logger;
    }

    public async Task<string> GetToken()
    {
        var providerOptions = _providerOptions.Value;
        var existingIdToken = await _jsRuntime.InvokeAsync<string>("getCurrentIdToken", $"oidc.user:{providerOptions.Authority}:{providerOptions.ClientId}");
        if (!string.IsNullOrEmpty(existingIdToken)) {
            _logger.LogInformation("ID token found.");
            
            if (!TokenCache.ContainsKey(existingIdToken))
            {
                _logger.LogInformation("Signing into game server...");
                var acquiredToken = await _authClient.LogInAsync(existingIdToken);
                TokenCache[existingIdToken] = acquiredToken;
            }
            
            return TokenCache[existingIdToken];
        }

        throw new Exception("Id token not found. Are we trying to authorize before authentication?");
    }
}