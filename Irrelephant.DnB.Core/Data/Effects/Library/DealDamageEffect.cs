using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;

namespace Irrelephant.DnB.Core.Data.Effects.Library
{
    public class DealDamageEffect : Effect
    {
        private readonly int _damage;

        public override string Name => $"Deal {_damage} damage.";

        public override Targets ValidTargets => Targets.SingleTarget | Targets.Enemy | Targets.Friendly;

        public override EffectType EffectType => EffectType.Debuff;

        public DealDamageEffect(int damage)
        {
            _damage = damage;
        }

        public async override Task Apply(IEnumerable<Character> targets)
        {
            var characters = targets as Character[] ?? targets.ToArray();
            await base.Apply(characters);
            characters.Single().DealDamage(_damage);
        }
    }
}
