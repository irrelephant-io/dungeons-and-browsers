using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.Characters
{
    public class PlayerCharacter : Character
    {
        public virtual int Energy { get; set; }

        public virtual int EnergyMax { get; set; }

        public virtual int DrawLimit { get; set; }

        public virtual IEnumerable<Card> DrawPile { get; set; } = Enumerable.Empty<Card>();

        public virtual IEnumerable<Card> DiscardPile { get; set; } = Enumerable.Empty<Card>();

        public virtual IEnumerable<Card> Hand { get; set; } = Enumerable.Empty<Card>();

        public virtual Task Discard(Card card)
        {
            // Can only discard card thats actually in hand
            var cardInHand = Hand.First(c => c == card);
            Hand = Hand.Where(c => c != cardInHand);
            DiscardPile = DiscardPile.Union(cardInHand.ArrayOf()).ToArray();

            // Setup for potential discard effects in the future
            return Task.CompletedTask;
        }

        public virtual Task Draw()
        {
            if (!DrawPile.Any())
            {
                DrawPile = DiscardPile.Shuffle().ToArray();
                DiscardPile = new Card[0];
            }

            if (!DrawPile.Any())
            {
                throw new NoCardsException();
            }

            var cardToDraw = DrawPile.First();
            Hand = cardToDraw.ArrayOf().Union(Hand).ToArray();
            DrawPile = DrawPile.Where(c => c != cardToDraw).ToArray();
            return Task.CompletedTask;
        }

        public async Task Draw(int count)
        {
            await Enumerable
                .Range(0, count)
                .Sequentially(_ => Draw());
        }
    }
}
