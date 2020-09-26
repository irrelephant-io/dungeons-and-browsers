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

        public virtual IList<CharacterController> Attackers { get; set; }

        public IList<(int pos, CharacterController cc)> PendingAttackers { get; protected set; } = new List<(int pos, CharacterController cc)>();

        public virtual IList<CharacterController> Defenders { get; set; }

        public IList<(int pos, CharacterController cc)> PendingDefenders { get; protected set; } = new List<(int pos, CharacterController cc)>();

        public IEnumerable<CharacterController> PendingCombatants => PendingAttackers.Union(PendingDefenders).Select(pending => pending.cc);

        public IEnumerable<CharacterController> Combatants => Attackers.Union(Defenders);

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

        private CharacterController FindController(Func<CharacterController, bool> predicate)
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

        public async Task AddAttacker(int position, CharacterController controller)
        {
            if (IsStarted)
            {
                await controller.JoinPendingCombat(JoinedSide.Attackers, position);
                PendingAttackers.Add((position, controller));
            }
            else
            {
                Attackers.Insert(position, controller);
            }
        }

        public async Task AddDefender(int position, CharacterController controller)
        {
            if (IsStarted)
            {
                await controller.JoinPendingCombat(JoinedSide.Defenders, position);
                PendingDefenders.Add((position, controller));
            }
            else
            {
                Defenders.Insert(position, controller);
            }
        }

        private async Task JoinPendingCombatants()
        {
            await JoinSide(Attackers, PendingAttackers);
            await JoinSide(Defenders, PendingDefenders);
        }

        private async Task JoinSide(IList<CharacterController> side, IList<(int pos, CharacterController cc)> buffer)
        {
            foreach (var pending in buffer)
            {
                side.Insert(pending.pos, pending.cc);
                pending.cc.OnAction += NotifyUpdate;
                await pending.cc.JoinCombat(ReferenceEquals(side, Attackers) ? JoinedSide.Attackers : JoinedSide.Defenders, pending.pos);
            }
            buffer.Clear();
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
            cleanedUpAttackers.ForEach(cc => Attackers.Remove(cc));
            var cleanedUpDefenders = Defenders.Where(combatant => !combatant.Character.IsAlive).ToArray();
            cleanedUpDefenders.ForEach(cc => Defenders.Remove(cc));
            await Cleanup(cleanedUpDefenders.Union(cleanedUpAttackers).ToArray());
        }

        private async Task Cleanup(IEnumerable<CharacterController> cleanedUpChars)
        {
            await cleanedUpChars.Sequentially(cc => cc.LeaveCombat());
        }
    }
}