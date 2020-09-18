using System.Collections.Generic;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Infrastructure;

namespace Irrelephant.DnB.Core.Data.Effects
{
    public class IdleEffect : Effect
    {
        public override string Name => string.Empty;

        public override Targets ValidTargets => Targets.Self;

        public override Task Apply(IEnumerable<Character> targets)
        {
            return Task.CompletedTask;
        }
    }
}
