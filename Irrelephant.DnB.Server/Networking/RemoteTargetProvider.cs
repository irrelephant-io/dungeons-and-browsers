using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.GameFlow;

namespace Irrelephant.DnB.Server.Networking
{
    public class RemoteTargetProvider : ITargetProvider
    {
        private readonly Combat _combat;
        private readonly IDictionary<Guid, Guid[]> _pickedEffectIds;

        public RemoteTargetProvider(Combat combat, IDictionary<Guid, Guid[]> cardTargets)
        {
            _combat = combat;
            _pickedEffectIds = cardTargets;
        }

        public Task<IEnumerable<Character>> PickTarget(Effect e)
        {
            var pickedTargetsForEffect = _pickedEffectIds[e.Id];
            return Task.FromResult(_combat
                .Combatants.Where(cc => pickedTargetsForEffect.Contains(cc.Character.Id))
                .Select(cc => cc.Character));
        }
    }
}
