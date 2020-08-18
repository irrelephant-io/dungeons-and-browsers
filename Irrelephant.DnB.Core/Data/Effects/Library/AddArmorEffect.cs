using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;

namespace Irrelephant.DnB.Core.Data.Effects.Library
{
    public class AddArmorEffect : Effect
    {
        private readonly int _armor;

        public override string Name => $"Gain {_armor} armor.";

        public override Targets ValidTargets => Targets.Self;

        public override EffectType EffectType => EffectType.Buff;

        public AddArmorEffect(int armor)
        {
            _armor = armor;
        }

        public async override Task Apply(IEnumerable<Character> targets)
        {
            var characters = targets as Character[] ?? targets.ToArray();
            await base.Apply(characters);
            targets.Single().Armor += _armor;
        }
    }
}