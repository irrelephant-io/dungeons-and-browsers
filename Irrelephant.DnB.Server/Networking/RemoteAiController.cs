using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Server.Hubs;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Networking
{
    public class RemoteAiController : AiController
    {
        private readonly IHubContext<CombatHub> _combatHubContext;

        public RemoteAiController(IHubContext<CombatHub> combatHubContext, Character character) : base(character)
        {
            _combatHubContext = combatHubContext;
        }

        public async override Task Act(Combat combat)
        {
            await base.Act(combat);
            await _combatHubContext.Clients.All.SendAsync("CharacterUpdated", Character.GetCharacterSnapshot());
        }
    }
}
