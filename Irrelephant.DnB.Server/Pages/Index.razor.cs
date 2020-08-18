using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Infrastructure;
using Irrelephant.DnB.Server.Components;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Server.Pages
{
    public partial class Index : ComponentBase
    {
        private CombatView _combatView;

        private ControlSurface _controlSurface;

        protected override void OnInitialized()
        {
            var playerCharacter = new PlayerCharacterController(new PlayerCharacter
            {
                Id = "player-0",
                Name = "Player",
                MaxHealth = 70,
                Health = 70
            });
            var combat = new Combat
            {
                Attackers = new CharacterController[]
                {
                    playerCharacter,
                    new AiController(new NonPlayerCharacter {Id = "goblin-0", Name = "Vile Goblin", Health = 30, MaxHealth = 30 }),
                },
                Defenders = new[]
                {
                    new AiController(new NonPlayerCharacter {Id = "orc-0", Name = "Raging Orc", Health = 55, MaxHealth = 55 }),
                    new AiController(new NonPlayerCharacter {Id = "goblin-1", Name = "Wretched Goblin", Health = 25, MaxHealth = 25 })
                },
                Log = new GameLog()
            };

            _combatView.Combat = combat;
            _controlSurface.Combat = combat;
            _controlSurface.Controller = playerCharacter;
        }
    }
}
