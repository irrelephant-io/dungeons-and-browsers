using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Server.Hubs;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Networking
{
    public class RemoteEffect : Effect
    {
        public override Guid Id => _effect.Id;

        public override Targets ValidTargets => _effect.ValidTargets;

        public override EffectType EffectType => _effect.EffectType;

        public override string Name => _effect.Name;

        private readonly IHubContext<CombatHub> _combatHubContext;

        private readonly Effect _effect;

        public RemoteEffect(IHubContext<CombatHub> combatHubContext, Effect effect)
        {
            _combatHubContext = combatHubContext;
            _effect = effect;
        }

        public async override Task Apply(IEnumerable<Character> targets)
        {
            var targetArray = targets.ToArray();
            await _effect.Apply(targetArray);
            await Task.WhenAll(targetArray.Select(c => _combatHubContext.Clients.All.SendAsync("CharacterUpdated", c.GetCharacterSnapshot())));
        }
    }
}
