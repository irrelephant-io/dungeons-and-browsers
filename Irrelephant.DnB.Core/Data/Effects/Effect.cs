using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;

namespace Irrelephant.DnB.Core.Data.Effects
{
    public class Effect
    {
        public virtual Guid Id { get; } = Guid.NewGuid();

        public virtual string Name { get; set; }

        public virtual Targets ValidTargets { get; } = Targets.None;

        public virtual EffectType EffectType { get; } = EffectType.Neutral;

        public virtual Task Apply(IEnumerable<Character> targets)
        {
            return Task.CompletedTask;
        }
    }
}
