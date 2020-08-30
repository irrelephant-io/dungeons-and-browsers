using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Infrastructure;
using Irrelephant.DnB.Core.Utils;
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
            }
        }

        private void SetupCombat()
        {
            _player = new PlayerCharacterController(SetupPlayer());
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

            _combat.Start();
            _combat.OnActionTaken += () => StateHasChanged();
        }

        private static PlayerCharacter SetupPlayer()
        {
            var playerHand = new Card
            {
                GraphicId = "resolute-strike",
                Name = "Resolute Strike",
                ActionCost = 2,
                Effects = new[] { EffectLibrary.Card.AddBlock, EffectLibrary.Card.DealSmallMeleeDamage }
            }.Copies(3)
            .Union(
                new Card
                {
                    GraphicId = "shiv-throw",
                    Name = "Shiv Throw",
                    ActionCost = 1,
                    Effects = new[] { EffectLibrary.Card.DealSmallDamage }
                }.Copies(3)
            );
            return new PlayerCharacter
            {
                GraphicId = "player-0",
                Name = "Player",
                MaxHealth = 70,
                Health = 70,
                EnergyMax = 4,
                DrawLimit = 5,
                DiscardPile = playerHand
            };
        }
    }
}