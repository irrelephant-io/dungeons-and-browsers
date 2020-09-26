using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Pages
{
    [Authorize]
    public partial class Index
    {
        private NavigationManager NavigationManager { get; }

        [Parameter]
        public string CombatId { get; set; }

        private Guid? _verifiedCombatId;

        private bool IsReady { get; set; }

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