using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Pages
{
    [Authorize]
    public partial class Combat
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string CombatId { get; set; }

        private Guid? _verifiedCombatId;

        private bool IsReady { get; set; }

        protected override Task OnParametersSetAsync()
        {
            _verifiedCombatId = GetCombatId();
            if (_verifiedCombatId == null)
            {
                NavigationManager.NavigateTo($"/combat/{Guid.NewGuid():D}");
                StateHasChanged();
            }
            else
            {
                IsReady = true;
                StateHasChanged();
            }
            
            return base.OnParametersSetAsync();
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