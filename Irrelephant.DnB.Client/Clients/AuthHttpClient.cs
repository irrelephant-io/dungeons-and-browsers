using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Irrelephant.DnB.Client.Clients
{
    public class AuthHttpClient : IAuthHttpClient
    {
        private readonly HttpClient _httpClient;

        public AuthHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LogInAsync(string idToken)
        {
            var content = new StringContent($"\"{idToken}\"", Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("/api/auth/login", content);
            return await result.Content.ReadAsStringAsync();
        }
    }
}
