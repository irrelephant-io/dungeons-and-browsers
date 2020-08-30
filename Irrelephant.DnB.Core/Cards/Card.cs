using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.Core.Infrastructure;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.Cards
{
    public class Card : ICopyable<Card>
    {
        public string GraphicId { get; set; }

        public string Name { get; set; }

        public string Text => string.Join(Environment.NewLine, Effects.Select(e => e.Name));

        public int ActionCost { get; set; }

        public IEnumerable<Effect> Effects { get; set; }

        public async Task Play(PlayerCharacter player, ITargetProvider targetProvider)
        {
            if (player.Energy >= ActionCost)
            {
                player.Energy -= ActionCost;
                await Effects.Sequentially(async e => await e.Apply(await targetProvider.PickTarget(e)));
                await player.Discard(this);
            }
            else
            {
                throw new NotEnoughActionsException();
            }
        }

        public bool CanPlay(PlayerCharacter player)
        {
            var isEnoughEnergy = player.Energy >= ActionCost;
            var isInHand = player.Hand.Contains(this);
            return isEnoughEnergy && isInHand;
        }

        public Card Copy()
        {
            var cardCopy = (Card)MemberwiseClone();
            return cardCopy;
        }
    }
}
