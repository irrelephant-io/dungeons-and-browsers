using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Server.Components
{
    public partial class ControlSurface : ComponentBase
    {
        public Combat Combat { get; set; }

        public PlayerCharacterController Controller { get; set; }

        public void EndTurn()
        {
            Controller.EndTurn();
        }
    }
}
