using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Server.Hubs;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Networking
{
    public class RemoteNonPlayerCharacter : NonPlayerCharacter
    {
        private readonly IHubContext<CombatHub> _combatHubContext;

        private IClientProxy HubClient => _combatHubContext.Clients.All;

        private IEnumerable<Effect> _effects;

        public override IEnumerable<Effect> ActionPool {
            get => _effects;
            set {
                _effects = value.Select(actionEffect => new RemoteEffect(_combatHubContext, actionEffect));
            }
        }

        public RemoteNonPlayerCharacter(IHubContext<CombatHub> combatHubContext)
        {
            _combatHubContext = combatHubContext;
        }

        public async override Task DealDamage(int amount, bool ignoreArmor = false)
        {
            await base.DealDamage(amount, ignoreArmor);
            await HubClient.SendAsync("CharacterUpdated", this.GetCharacterSnapshot());
        }
    }
}
