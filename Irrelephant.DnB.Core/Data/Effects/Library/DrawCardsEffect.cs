using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Infrastructure;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.Data.Effects.Library
{
    public class DrawCardsEffect : Effect
    {
        private readonly int _cards;

        public override string Name => _cards > 1 ? $"Draw {_cards} cards." : "Draw a card.";

        public override Targets ValidTargets => Targets.Self;

        public override EffectType EffectType => EffectType.Buff;

        public DrawCardsEffect(int cards)
        {
            _cards = cards;
        }

        public async override Task Apply(IEnumerable<Character> targets)
        {
            var characterArray = targets as Character[] ?? targets.ToArray();
            await base.Apply(characterArray);
            characterArray.ForEach(c => (c as PlayerCharacter)?.Draw(_cards));
        }
    }
}