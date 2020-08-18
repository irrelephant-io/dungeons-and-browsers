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

        public void EndTurn()
        {
            Controller.EndTurn();
        }
    }
}
