using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Infrastructure;

namespace Irrelephant.DnB.Core.Data.Effects.Library
{
    public class DealDamageToSelfEffect : DealDamageEffect
    {
        public override string Name => $"Deal {Damage} damage to self.";

        public override Targets ValidTargets => Targets.Self;

        public override EffectType EffectType => EffectType.Debuff;

        public DealDamageToSelfEffect(int damage) : base(damage)
        {
        }
    }
}
