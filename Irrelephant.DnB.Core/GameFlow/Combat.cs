using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.GameFlow
{
    public class Combat
    {
        public Guid CombatId { get; set; }

        public virtual IEnumerable<CharacterController> Attackers { get; set; }

        public virtual IList<(int pos, CharacterController cc)> PendingAttackers { get; set; } = new List<(int pos, CharacterController cc)>();

        public virtual IEnumerable<CharacterController> Defenders { get; set; }

        public virtual IList<(int pos, CharacterController cc)> PendingDefenders { get; set; } = new List<(int pos, CharacterController cc)>();

        public virtual IEnumerable<CharacterController> PendingCombatants => PendingAttackers.Union(PendingDefenders).Select(pending => pending.cc);

        public virtual IEnumerable<CharacterController> Combatants => Attackers.Union(Defenders);

        public CharacterController CurrentActiveCharacter { get; private set; }

        public bool IsStarted { get; private set; }

        public int Round { get; private set; } = 1;

        public bool IsOver => !Attackers.Any() || !Defenders.Any();

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
            return Attackers.FirstOrDefault(predicate) 
                   ?? Defenders.FirstOrDefault(predicate)
                   ?? PendingAttackers.FirstOrDefault(pair => predicate(pair.cc)).cc
                   ?? PendingDefenders.FirstOrDefault(pair => predicate(pair.cc)).cc;
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
            await Start();
            while (!IsOver)
            {
                await ResolveRound();
            }
        }

        public async Task ResolveRound()
        {
            await Combatants.Sequentially(RunCombatantTurn);
            await CleanupDeadBodies();
            await JoinPendingCombatants();
            Round++;
        }

        public Task AddAttacker(int position, CharacterController controller)
        {
            if (IsStarted)
            {
                controller.JoinPendingCombat(JoinedSide.Attackers, position);
                PendingAttackers.Add((position, controller));
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
                controller.JoinPendingCombat(JoinedSide.Defenders, position);
                PendingDefenders.Add((position, controller));
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
            Attackers = await JoinSide(Attackers, PendingAttackers);
            Defenders = await JoinSide(Defenders, PendingDefenders);
        }

        private async Task<CharacterController[]> JoinSide(IEnumerable<CharacterController> side, IList<(int pos, CharacterController cc)> buffer)
        {
            var sideList = side.ToList();
            foreach (var pending in buffer)
            {
                sideList.Insert(pending.pos, pending.cc);
                pending.cc.OnAction += NotifyUpdate;
                await pending.cc.JoinCombat(side == Attackers ? JoinedSide.Attackers : JoinedSide.Defenders, pending.pos);
            }
            buffer.Clear();
            return sideList.ToArray();
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
            playerCharacter.Actions = playerCharacter.ActionsMax;
            playerCharacter.Armor = 0;
            await playerCharacter.DrawHand();
            await controller.Act(this);
            await playerCharacter.DiscardHand();
        }

        private async Task CleanupDeadBodies()
        {
            var cleanedUpAttackers = Attackers.Where(combatant => !combatant.Character.IsAlive).ToArray();
            Attackers = Attackers.Where(combatant => combatant.Character.IsAlive).ToArray();
            var cleanedUpDefenders = Defenders.Where(combatant => !combatant.Character.IsAlive).ToArray();
            Defenders = Defenders.Where(combatant => combatant.Character.IsAlive).ToArray();
            await Cleanup(cleanedUpDefenders.Union(cleanedUpAttackers).ToArray());
        }

        private async Task Cleanup(IEnumerable<CharacterController> cleanedUpChars)
        {
            await cleanedUpChars.Sequentially(cc => cc.LeaveCombat());
        }
    }
}