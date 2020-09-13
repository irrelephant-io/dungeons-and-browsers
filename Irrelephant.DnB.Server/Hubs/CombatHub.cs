using System.Threading.Tasks;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Hubs
{
    public class CombatHub : Hub<ICombatClient>
    {
        private Combat _combat;

        public CombatHub()
        {
            _combat = CombatFactory.BuildCombat();
        }

        public async Task JoinCombat()
        {
            await Clients.Caller.Joined(_combat.GetSnapshot());
        }

        public async Task EndTurn()
        {
            await Task.Delay(1000);
            await Clients.Caller.MyTurn();
        }
    }
}
