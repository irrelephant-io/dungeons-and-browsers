using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Infrastructure;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.GameFlow
{
    public class Combat
    {
        public virtual IEnumerable<CharacterController> Attackers { get; set; }

        public virtual IEnumerable<CharacterController> Defenders { get; set; }

        public IEffector Effector { get; set; }

        public int Round { get; private set; } = 1;

        public event Action OnActionTaken;


        protected Character FindCharacterById(Guid id)
        {
            return (Attackers.FirstOrDefault(a => a.Character.Id == id)
                    ?? Defenders.FirstOrDefault(d => d.Character.Id == id))
                ?.Character;
        }

        public void Start()
        {
            Attackers.ForEach(a => a.OnAction += () => OnActionTaken?.Invoke());
            Defenders.ForEach(d => d.OnAction += () => OnActionTaken?.Invoke());
        }

        public async Task ResolveRound()
        {
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