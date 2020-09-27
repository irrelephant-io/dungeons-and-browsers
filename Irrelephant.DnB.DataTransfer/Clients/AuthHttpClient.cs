using System.Net.Http;
using System.Threading.Tasks;

namespace Irrelephant.DnB.DataTransfer.Clients
{
    public class AuthHttpClient : ApiClientBase, IAuthHttpClient
    {
        public AuthHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public Task<string> LogInAsync(string idToken)
        {
            return PostJsonAsync<string, string>("/api/auth/login", idToken);
        }
    }
}
