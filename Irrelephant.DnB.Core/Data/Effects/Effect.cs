using System.Collections.Generic;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Infrastructure;

namespace Irrelephant.DnB.Core.Data.Effects
{
    public class Effect
    {
        public virtual string Name { get; set; }

        public virtual Targets ValidTargets { get; } = Targets.None;

        public virtual EffectType EffectType { get; } = EffectType.Neutral;

        public async virtual Task Apply(IEnumerable<Character> targets, IEffector effector = null)
        {
            var task = effector?.CreateDelay(timeMs: 1000);
            if (task != null)
            {
                await task;
            }
        }
    }
}
