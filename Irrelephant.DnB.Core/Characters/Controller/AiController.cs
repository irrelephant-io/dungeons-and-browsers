using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.Characters.Controller
{
    public class AiController : CharacterController
    {
        private Queue<Effect> _actionQueue;

        public AiController(Character character) : base(character)
        {
            if (!(character is NonPlayerCharacter))
            {
                throw new NotImplementedException("AI controller can only control NPCs.");
            }

            SetupActionQueue(character);
        }

        private void SetupActionQueue(Character character)
        {
            var npc = (NonPlayerCharacter) character;
            var actions = npc.ActionPool ?? new[] {new IdleEffect()}.ToArray();
            _actionQueue = new Queue<Effect>(actions);
        }

        public async override Task Act(Combat combat)
        {
            var nextAction = _actionQueue.Dequeue();
            await nextAction.Apply(PickTarget(nextAction, combat));
            _actionQueue.Enqueue(nextAction);
        }

        private IEnumerable<Character> PickTarget(Effect nextAction, Combat combat)
        {
            if (nextAction.ValidTargets == Targets.Self)
            {
                return new[] {Character};
            }

            if (nextAction.ValidTargets == Targets.All)
            {
                return combat.Defenders
                    .Select(cc => cc.Character)
                    .Union(combat.Attackers
                        .Select(cc => cc.Character));
            }

            if ((nextAction.ValidTargets & Targets.Team) != 0)
            {
                if ((nextAction.ValidTargets & Targets.Friendly) != 0)
                {
                    return this.GetTeamIn(combat).Select(cc => cc.Character);
                }

                if ((nextAction.ValidTargets & Targets.Enemy) != 0)
                {
                    return this.GetOpposingTeamIn(combat).Select(cc => cc.Character);
                }
            }

            if ((nextAction.ValidTargets & Targets.SingleTarget) != 0)
            {
                if ((nextAction.ValidTargets & Targets.Friendly) != 0)
                {
                    return this.GetTeamIn(combat).Take(1).Select(cc => cc.Character);
                }

                if ((nextAction.ValidTargets & Targets.Enemy) != 0)
                {
                    return this.GetOpposingTeamIn(combat).Take(1).Select(cc => cc.Character);
                }
            }

            return Enumerable.Empty<Character>();
        }
    }
}
