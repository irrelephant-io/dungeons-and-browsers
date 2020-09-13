using System.Threading.Tasks;
using Irrelephant.DnB.Client.Networking;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Pages
{
    public partial class Index : ComponentBase
    {
        public bool IsReady;

        public PlayerCharacterController _pc = new PlayerCharacterController(new PlayerCharacter());

        private ClientCombat _combat;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var network = new RemoteCombatListener(Navigation);
                await network.StartAsync();
                _combat = new ClientCombat(network);
                _combat.OnUpdate += StateHasChanged;
                IsReady = true;
                StateHasChanged();
            }
        }
    }
}