using System;
using Irrelephant.DnB.Client.Networking;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Shared
{
    public partial class CombatView : ComponentBase
    {
        [Parameter] public ClientCombat Combat { get; set; }

        [CascadingParameter] public ControlSurface ControlSurface { get; set; }

        protected override void OnInitialized()
        {
            if (Combat != null)
            {
                Console.WriteLine("Wathcing combat " + Combat.CombatId);
            }
        }
    }
}
