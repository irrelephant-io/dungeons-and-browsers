using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Client.Networking;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Pages
{
    public partial class Index : ComponentBase
    {
        public bool IsReady;
        
        private ClientCombat _combat;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Console.WriteLine("Creating combat");
                var network = new RemoteCombatListener(Navigation);
                await network.StartAsync();
                _combat = new ClientCombat(network);
                _combat.OnUpdate += StateHasChanged;
                IsReady = true;
                Console.WriteLine("All good");
                StateHasChanged();
            }
        }
    }
}