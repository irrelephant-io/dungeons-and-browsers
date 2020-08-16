using System.Collections.Generic;
using Irrelephant.DnB.Core.Cards;

namespace Irrelephant.DnB.Core.Characters
{
    public class PlayerCharacter : Character
    {
        public IEnumerable<Card> DrawPile;

        public IEnumerable<Card> DiscardPile;

        public IEnumerable<Card> Hand;

        public override void TakeTurn()
        {

        }
    }
}
