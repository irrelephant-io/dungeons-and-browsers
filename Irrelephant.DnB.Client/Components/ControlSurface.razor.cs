using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Utils;
using Microsoft.AspNetCore.Components;

namespace Irrelephant.DnB.Client.Components
{
    public partial class ControlSurface : ComponentBase, ITargetProvider
    {
        [Parameter]
        public Combat Combat { get; set; }

        [Parameter]
        public PlayerCharacterController Controller { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public Card PlayedCard { get; set; }

        public PlayerCharacter Player => (PlayerCharacter)Controller.Character;

        private TaskCompletionSource<IEnumerable<Character>> _targetPickingPromise;

        private string _targetPickingClass = string.Empty;

        public async Task EndTurn()
        {
            await Controller.EndTurn();
        }

        public Task<IEnumerable<Character>> PickTarget(Effect e)
        {
            if (e.ValidTargets.Matches(Targets.Self))
            {
                return Task.FromResult(Player.ArrayOf<Character>());
            }

            if (e.ValidTargets.Matches(Targets.MeleeRange))
            {
                return Task.FromResult(Controller.GetOpposingTeamIn(Combat).First().Character.ArrayOf());
            }

            if (e.ValidTargets.Matches(Targets.Team))
            {
                var result = e.ValidTargets.Matches(Targets.Enemy)
                    ? Controller.GetOpposingTeamIn(Combat).Select(cc => cc.Character)
                    : Controller.GetTeamIn(Combat).Select(cc => cc.Character);
                return Task.FromResult(result);
            }

            if (e.ValidTargets.Matches(Targets.SingleTarget))
            {
                _targetPickingPromise = new TaskCompletionSource<IEnumerable<Character>>();
                PickSingleTarget(e.ValidTargets);
                System.Console.WriteLine("Picking target");
                return _targetPickingPromise.Task;
            }

            return Task.FromResult(Enumerable.Empty<Character>());
        }

        private void PickSingleTarget(Targets validTargets)
        {
            if (validTargets.Matches(Targets.Enemy))
            {
                if (Combat.Attackers.Contains(Controller))
                    _targetPickingClass += "select-defender ";
                else
                    _targetPickingClass += "select-attacker ";
            }

            if (validTargets.Matches(Targets.Friendly))
            {
                if (Combat.Attackers.Contains(Controller))
                    _targetPickingClass += "select-attacker ";
                else
                    _targetPickingClass += "select-defender ";
            }

            StateHasChanged();
        }

        private string GetRotationStyle(int index)
        {
            var halfSize = Player.Hand.Count() / 2;
            var adjustedIndex = index - halfSize;
            if (adjustedIndex < 0)
            {
                return $"rotate-left-{-adjustedIndex}";
            }

            if (adjustedIndex > 0)
            {
                return $"rotate-right-{adjustedIndex}";
            }

            return "rotate-none";
        }

        private async Task HandleCardDrop()
        {
            if (PlayedCard.CanPlay(Player)) {
                await PlayedCard.Play(Player, this);
                Controller.InvokeOnAction();
            }
            else {
                System.Console.WriteLine("Cant play the card!");
            }
        }

        private Task HandleDragEnter()
        {
            System.Console.WriteLine("Entered!");
            return Task.CompletedTask;
        }

        private Task HandleDragLeave()
        {
            System.Console.WriteLine("Left!");
            return Task.CompletedTask;
        }

        public void CardClicked(Card c)
        {
            // Nothing for now
        }

        public void CharacterClicked(Character c)
        {
            System.Console.WriteLine("Clicked");
            if (_targetPickingPromise != null)
            {
                _targetPickingPromise.SetResult(c.ArrayOf());
                _targetPickingClass = string.Empty;
            }
        }
    }
}
