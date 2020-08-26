using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Components
{
    public partial class ControlSurface : ComponentBase
    {
        [Parameter]
        public Combat Combat { get; set; }

        [Parameter]
        public PlayerCharacterController Controller { get; set; }

        public Card PlayedCard { get; set; }

        public PlayerCharacter Player => (PlayerCharacter)Controller.Character;

        public void EndTurn()
        {
            Controller.EndTurn();
        }

        private string GetRotationStyle(int index)
        {
            var halfSize = Player.Hand.Count() / 2;
            var adjustedIndex = index - halfSize;
            if (adjustedIndex < 0)
            {
                return $"rotate-left-{-adjustedIndex}";
            }

            if (adjustedIndex > 0)
            {
                return $"rotate-right-{adjustedIndex}";
            }

            return "rotate-none";
        }

        private async Task HandleCardDrop()
        {
            if (PlayedCard.CanPlay(Player)) {
                await PlayedCard.Play(Player, null);
            }
            else {
                System.Console.WriteLine("Cant play the card!");
            }
        }

        private Task HandleDragEnter()
        {
            System.Console.WriteLine("Entered!");
            return Task.CompletedTask;
        }

        private Task HandleDragLeave()
        {
            System.Console.WriteLine("Left!");
            return Task.CompletedTask;
        }
    }
}
