using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.Data.Effects.Library
{
    public class DealDamageEffect : Effect
    {
        protected readonly int Damage;

        public override string Name => $"Deal {Damage} damage.";

        public override Targets ValidTargets => Targets.SingleTarget | Targets.Enemy | Targets.Friendly;

        public override EffectType EffectType => EffectType.Debuff;

        public DealDamageEffect(int damage)
        {
            Damage = damage;
        }

        public async override Task Apply(IEnumerable<Character> targets)
        {
            var characters = targets as Character[] ?? targets.ToArray();
            await base.Apply(characters);
            await characters.Sequentially(c => c.DealDamage(Damage));
        }
    }
}
