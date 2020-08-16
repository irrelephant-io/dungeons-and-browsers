using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Server.Components
{
    public partial class CombatView : ComponentBase
    {
        private readonly Combat _combat;

        public CombatView()
        {
            _combat = new Combat
            {
                Attackers = new[]
                {
                    new AiController(new NonPlayerCharacter {Id = "goblin-0", Name = "Vile Goblin", Health = 30, MaxHealth = 30 }),
                    new AiController(new NonPlayerCharacter {Id = "goblin-1", Name = "Wretched Goblin", Health = 25, MaxHealth = 25 })
                },
                Defenders = new[] {new AiController(new NonPlayerCharacter {Id="orc-0", Name = "Raging Orc", Health = 55, MaxHealth = 55 }),}
            };
        }
    }
}
