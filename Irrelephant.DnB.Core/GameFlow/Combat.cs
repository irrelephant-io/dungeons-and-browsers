using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.GameFlow
{
    public class Combat
    {
        public virtual IEnumerable<CharacterController> Attackers { get; set; }

        public virtual IEnumerable<CharacterController> Defenders { get; set; }

        public int Round { get; private set; } = 1;

        public async Task ResolveRound()
        {
            await Attackers.Sequentially(attacker => attacker.Act(this));
            await Defenders.Sequentially(attacker => attacker.Act(this));
            Round++;
        }

        public bool IsOver => !Attackers.Any() || !Defenders.Any();
    }
}