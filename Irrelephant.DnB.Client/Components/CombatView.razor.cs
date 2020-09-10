using System.Threading.Tasks;
using Irrelephant.DnB.Core.GameFlow;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Components
{
    public partial class CombatView : ComponentBase
    {
        [Parameter]
        public Combat Combat { get; set; }

        [CascadingParameter]
        public ControlSurface ControlSurface { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Combat.Effector = ControlSurface;
            }
        }
    }
}
