using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.Cards
{
    public class Card
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Text => string.Join(Environment.NewLine, Effects.Select(e => e.Name));

        public int ActionCost { get; set; }

        public IEnumerable<Effect> Effects { get; set; }

        public async Task Play(PlayerCharacter player, ITargetProvider targetProvider)
        {
            if (player.Energy >= ActionCost)
            {
                player.Energy -= ActionCost;
                await Effects.Sequentially(e => e.Apply(targetProvider.PickTarget(e)));
            }
            else
            {
                throw new NotEnoughActionsException();
            }
        }
    }
}
