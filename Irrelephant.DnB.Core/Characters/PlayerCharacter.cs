using System.Collections.Generic;
using Irrelephant.DnB.Core.Cards;

namespace Irrelephant.DnB.Core.Characters
{
    public class PlayerCharacter : Character
    {
        public virtual int Energy { get; set; }

        public virtual int EnergyMax { get; set; }

        public IEnumerable<Card> DrawPile;

        public IEnumerable<Card> DiscardPile;

        public IEnumerable<Card> Hand;

        public override void TakeTurn()
        {

        }
    }
}
