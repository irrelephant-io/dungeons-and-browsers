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
        public virtual Guid Id { get; set; }

        public virtual string GraphicId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Text => string.Join(Environment.NewLine, Effects.Select(e => e.Name));

        public virtual int ActionCost { get; set; }

        public virtual IEnumerable<Effect> Effects { get; set; }

        public async virtual Task Play(PlayerCharacter player, ITargetProvider targetProvider)
        {
            if (player.Actions >= ActionCost)
            {
                player.Actions -= ActionCost;
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
            var isEnoughEnergy = player.Actions >= ActionCost;
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
