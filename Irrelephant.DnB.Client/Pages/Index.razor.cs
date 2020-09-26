using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Client.Networking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Pages
{
    [Authorize]
    public partial class Index : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string CombatId { get; set; }

        private Guid? _verifiedCombatId { get; set; }

        public bool IsReady;

        protected override void OnParametersSet()
        {
            _verifiedCombatId = GetCombatId();
            if (_verifiedCombatId == null)
            {
                NavigationManager.NavigateTo(Guid.NewGuid().ToString("D"));
                StateHasChanged();
            }
            else
            {
                IsReady = true;
                StateHasChanged();
            }
        }

        private Guid? GetCombatId()
        {
            if (string.IsNullOrEmpty(CombatId))
            {
                return null;
            }

            if (Guid.TryParse(CombatId, out var guidId))
            {
                return guidId;
            }

            return null;
        }
    }
}