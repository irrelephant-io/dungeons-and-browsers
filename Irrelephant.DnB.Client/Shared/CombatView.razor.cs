using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Client.Networking;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Shared
{
    public partial class CombatView
    {
        [Parameter] public ClientCombat Combat { get; set; }

        [CascadingParameter] public ControlSurface ControlSurface { get; set; }

        protected override Task OnInitializedAsync()
        {
            if (Combat != null)
            {
                Console.WriteLine("Watching combat " + Combat.CombatId);
            }
            
            return base.OnInitializedAsync();
        }
    }
}
