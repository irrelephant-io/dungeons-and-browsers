using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Hubs
{
    public class CombatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
