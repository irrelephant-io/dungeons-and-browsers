using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Irrelephant.DnB.DataTransfer.Models;

namespace Irrelephant.DnB.DataTransfer.Clients
{
    public class LobbyHttpClient : ApiClientBase, ILobbyHttpClient
    {
        public LobbyHttpClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<IEnumerable<CombatInfo>> GetOngoingCombatsAsync()
        {
            return await GetJsonAsync<IEnumerable<CombatInfo>>("/api/lobby/combats");
        }
    }
}
