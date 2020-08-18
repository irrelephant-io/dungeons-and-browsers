using Irrelephant.DnB.Core.GameFlow;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Components
{
    public partial class CombatView : ComponentBase
    {
        [Parameter]
        public Combat Combat { get; set; }

    }
}
