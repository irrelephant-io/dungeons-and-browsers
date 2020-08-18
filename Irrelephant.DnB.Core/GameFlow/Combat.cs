using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Infrastructure;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.GameFlow
{
    public class Combat
    {
        public GameLog Log { get; set; }

        public virtual IEnumerable<CharacterController> Attackers { get; set; }

        public virtual IEnumerable<CharacterController> Defenders { get; set; }

        public int Round { get; private set; } = 1;

        public async Task ResolveRound()
        {
            Console.WriteLine($"Round {Round} starts!");
            await Attackers.Sequentially(attacker => attacker.Act(this));
            await Defenders.Sequentially(defender => defender.Act(this));
            CleanupDeadBodies();
            Round++;
        }

        private void CleanupDeadBodies()
        {
            Attackers = Attackers.Where(combatant => combatant.Character.IsAlive).ToArray();
            Defenders = Defenders.Where(combatant => combatant.Character.IsAlive).ToArray();
        }

        public bool IsOver => !Attackers.Any() || !Defenders.Any();
    }
}