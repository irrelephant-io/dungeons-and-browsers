using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Pages
{
    public partial class Index : ComponentBase
    {
        private bool _isReady;

        private PlayerCharacterController _player;

        private Combat _combat;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                SetupCombat();
                _isReady = true;
                StateHasChanged();
                await UpdateLoop();
            }
        }

        public async Task UpdateLoop()
        {
            while (!_combat.IsOver)
            {
                await _combat.ResolveRound();
                StateHasChanged();
            }
        }

        private void SetupCombat()
        {
            _player = new PlayerCharacterController(new PlayerCharacter
            {
                Id = "player-0",
                Name = "Player",
                MaxHealth = 70,
                Health = 70
            });
            _combat = new Combat
            {
                Attackers = new CharacterController[]
                {
                    _player,
                    new AiController(CharacterLibrary.VileGoblin)
                },
                Defenders = new[]
                {
                    new AiController(CharacterLibrary.RagingOrc),
                    new AiController(CharacterLibrary.WretchedGoblin)
                },
                Log = new GameLog()
            };
        }
    }
}