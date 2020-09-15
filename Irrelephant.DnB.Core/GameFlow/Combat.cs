using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.GameFlow
{
    public class Combat
    {
        public Guid CombatId { get; set; }

        public virtual IEnumerable<CharacterController> Attackers { get; set; }
        private IList<(int pos, CharacterController cc)> _pendingAttackers { get; set; } = new List<(int pos, CharacterController cc)>();

        public virtual IEnumerable<CharacterController> Defenders { get; set; }
        private IList<(int pos, CharacterController cc)> _pendingDefenders { get; set; } = new List<(int pos, CharacterController cc)>();

        public virtual IEnumerable<CharacterController> Combatants => Attackers.Union(Defenders);

        public CharacterController CurrentActiveCharacter { get; private set; }

        public bool IsStarted { get; private set; }

        public int Round { get; private set; } = 1;

        public event Action OnUpdate;

        protected void NotifyUpdate()
        {
            OnUpdate?.Invoke();
        }

        public Character FindCharacterById(Guid id)
        {
            return FindControllerByCharacterId(id)?.Character;
        }

        public CharacterController FindControllerByCharacterId(Guid id)
        {
            return FindController(cc => cc.Character.Id == id);
        }

        public CharacterController FindController(Func<CharacterController, bool> predicate)
        {
            return Attackers.FirstOrDefault(predicate) ?? Defenders.FirstOrDefault(predicate);
        }

        public Task Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                Attackers.ForEach(a => a.OnAction += NotifyUpdate);
                Defenders.ForEach(d => d.OnAction += NotifyUpdate);
            }

            return Task.CompletedTask;
        }

        public async Task RunCombat()
        {
            while (!IsOver)
            {
                await ResolveRound();
            }
        }

        public async Task ResolveRound()
        {
            await Combatants.Sequentially(RunCombatantTurn);
            CleanupDeadBodies();
            await JoinPendingCombatants();
            Round++;
        }

        public Task AddAttacker(int position, CharacterController controller)
        {
            if (IsStarted)
            {
                _pendingAttackers.Add((position, controller));
            }
            else
            {
                var attackerList = Attackers.ToList();
                attackerList.Insert(position, controller);
                Attackers = attackerList.ToArray();
            }
            return Task.CompletedTask;
        }

        public Task AddDefender(int position, CharacterController controller)
        {
            if (IsStarted)
            {
                _pendingDefenders.Add((position, controller));
            }
            else
            {
                var defenderList = Defenders.ToList();
                defenderList.Insert(position, controller);
                Defenders = defenderList.ToArray();
            }

            return Task.CompletedTask;
        }

        private async Task JoinPendingCombatants()
        {
            Attackers = await JoinSide(Attackers, _pendingAttackers);
            Defenders = await JoinSide(Defenders, _pendingDefenders);
        }

        private Task<CharacterController[]> JoinSide(IEnumerable<CharacterController> side, IList<(int pos, CharacterController cc)> buffer)
        {
            var sideList = side.ToList();
            foreach (var pending in buffer)
            {
                sideList.Insert(pending.pos, pending.cc);
                pending.cc.OnAction += NotifyUpdate;
            }
            buffer.Clear();
            return Task.FromResult(sideList.ToArray());
        }

        private async Task RunCombatantTurn(CharacterController characterController)
        {
            var character = characterController.Character;
            CurrentActiveCharacter = characterController;
            if (characterController is PlayerCharacterController pcController && character is PlayerCharacter playerCharacter)
            {
                await RunPlayerTurn(pcController, playerCharacter);
            }

            if (characterController is AiController aiController)
            {
                await aiController.Act(this);
            }
        }

        private async Task RunPlayerTurn(PlayerCharacterController controller, PlayerCharacter playerCharacter)
        {
            playerCharacter.Energy = playerCharacter.EnergyMax;
            playerCharacter.Armor = 0;
            await playerCharacter.DrawHand();
            await controller.Act(this);
            await playerCharacter.DiscardHand();
        }

        private void CleanupDeadBodies()
        {
            Attackers = Attackers.Where(combatant => combatant.Character.IsAlive).ToArray();
            Defenders = Defenders.Where(combatant => combatant.Character.IsAlive).ToArray();
        }

        public bool IsOver => !Attackers.Any() || !Defenders.Any();
    }
}