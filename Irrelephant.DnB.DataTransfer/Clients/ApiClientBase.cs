using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Irrelephant.DnB.DataTransfer.Clients
{
    public class ApiClientBase
    {
        private HttpClient Client { get; }

        protected ApiClientBase(HttpClient client)
        {
            Client = client;
        }

        protected async Task<TResult> GetJsonAsync<TResult>(string uri)
        {
            var result = await Client.GetAsync(uri);
            var stringResult = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResult>(stringResult);
        }

        protected async Task<TResult> PostJsonAsync<TResult, TPayload>(string uri, TPayload payload)
        {
            var payloadString = JsonSerializer.Serialize(payload);
            var postContent = new StringContent(payloadString, Encoding.UTF8, "application/json");
            var result = await Client.PostAsync(uri, postContent);
            var stringResult = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResult>(stringResult);
        }
    }
}
