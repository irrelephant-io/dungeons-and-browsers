using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;

namespace Irrelephant.DnB.Core.Data.Effects
{
    public class Effect
    {
        public virtual string Name { get; set; }

        public virtual Targets ValidTargets { get; } = Targets.None;

        public virtual EffectType EffectType { get; } = EffectType.Neutral;

        public virtual Task Apply(IEnumerable<Character> targets)
        {
            Console.WriteLine($"Effect '{Name}' is resolved. Targets: {string.Join(", ", targets.Select(t => t.Name))}");
            return Task.CompletedTask;
        }
    }
}
