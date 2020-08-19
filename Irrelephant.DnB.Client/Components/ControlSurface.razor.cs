using System.Linq;
using Irrelephant.DnB.Client.Pages;
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

        private PlayerCharacter Player => (PlayerCharacter)Controller.Character;

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
    }
}
