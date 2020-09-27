using System.Collections.Generic;
using System.Threading.Tasks;
using Irrelephant.DnB.DataTransfer.Models;

namespace Irrelephant.DnB.DataTransfer.Clients
{
    public interface ILobbyHttpClient
    {
        Task<IEnumerable<CombatInfo>> GetOngoingCombatsAsync();
    }
}
