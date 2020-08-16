using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.GameFlow
{
    public class Combat
    {
        public IEnumerable<CharacterController> Attackers;

        public IEnumerable<CharacterController> Defenders;

        public int Round { get; private set; } = 1;

        public async Task ResolveRound()
        {
            await Attackers.Sequentially(attacker => attacker.Act());
            await Defenders.Sequentially(attacker => attacker.Act());
            Round++;
        }

        public bool IsDone => !Attackers.Any() || !Defenders.Any();
    }
}