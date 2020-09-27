using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Irrelephant.DnB.Server.Tests.Infrastructure
{
    internal static class TestTokenProvider
    {
        private static string _acquiredToken;

        public static async Task<string> GetToken(HttpClient testClient)
        {
            if (string.IsNullOrEmpty(_acquiredToken))
            {
                var pretendIdToken = new StringContent("\"I AM A GOOGLE ID TOKEN WANNABE\"", Encoding.UTF8, "application/json");
                var result = await testClient.PostAsync("/api/auth/login", pretendIdToken);
                _acquiredToken = (await result.Content.ReadAsStringAsync()).Trim('"');
            }

            return _acquiredToken;
        }
    }
}
