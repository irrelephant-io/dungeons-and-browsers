using System.Threading.Tasks;
using Irrelephant.DnB.Core.Networking;

namespace Irrelephant.DnB.Server.Hubs
{
    public interface ICombatClient
    {
        Task Joined(CombatSnapshot snapshot);

        Task MyTurn();
    }
}
