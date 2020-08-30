using System;
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
            await base.Act(combat);
            var nextAction = _actionQueue.Dequeue();
            await nextAction.Apply(PickTarget(nextAction, combat));
            _actionQueue.Enqueue(nextAction);
            InvokeOnAction();
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

            if (nextAction.ValidTargets.Matches(Targets.MeleeRange))
            {
                return this.GetOpposingTeamIn(combat).Take(1).Select(cc => cc.Character);
            }

            if (nextAction.ValidTargets.Matches(Targets.Team))
            {
                return PickTeamTarget(nextAction, combat);
            }

            if (nextAction.ValidTargets.Matches(Targets.SingleTarget))
            {
                return PickSingleTarget(nextAction, combat);
            }

            return Enumerable.Empty<Character>();
        }

        private IEnumerable<Character> PickSingleTarget(Effect nextAction, Combat combat)
        {
            if (nextAction.ValidTargets.Matches(Targets.Friendly, Targets.Enemy))
            {
                if (nextAction.EffectType == EffectType.Buff)
                {
                    return this.GetTeamIn(combat).Take(1).Select(cc => cc.Character);
                }
                if (nextAction.EffectType == EffectType.Debuff)
                {
                    return this.GetOpposingTeamIn(combat).Take(1).Select(cc => cc.Character);
                }
            }

            if (nextAction.ValidTargets.Matches(Targets.Friendly))
            {
                return this.GetTeamIn(combat).Take(1).Select(cc => cc.Character);
            }

            if (nextAction.ValidTargets.Matches(Targets.Enemy))
            {
                return this.GetOpposingTeamIn(combat).Take(1).Select(cc => cc.Character);
            }

            return Enumerable.Empty<Character>();
        }

        private IEnumerable<Character> PickTeamTarget(Effect nextAction, Combat combat)
        {
            if (nextAction.ValidTargets.Matches(Targets.Friendly, Targets.Enemy))
            {
                if (nextAction.EffectType == EffectType.Buff)
                {
                    return this.GetTeamIn(combat).Select(cc => cc.Character);
                }
                if (nextAction.EffectType == EffectType.Debuff)
                {
                    return this.GetOpposingTeamIn(combat).Select(cc => cc.Character);
                }
            }

            if (nextAction.ValidTargets.Matches(Targets.Friendly))
            {
                return this.GetTeamIn(combat).Select(cc => cc.Character);
            }

            if (nextAction.ValidTargets.Matches(Targets.Enemy))
            {
                return this.GetOpposingTeamIn(combat).Select(cc => cc.Character);
            }

            return Enumerable.Empty<Character>();
        }
    }
}
