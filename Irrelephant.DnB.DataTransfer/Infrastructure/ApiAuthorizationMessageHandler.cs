using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Irrelephant.DnB.DataTransfer.Services;

namespace Irrelephant.DnB.DataTransfer.Infrastructure
{
    public class ApiAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IApiTokenProvider _tokenProvider;

        public ApiAuthorizationMessageHandler(IApiTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenProvider.GetToken());
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
