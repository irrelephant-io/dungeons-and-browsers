using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Core.Utils;

namespace Irrelephant.DnB.Core.Characters.Controller
{
    public class PlayerCharacterController : CharacterController
    {
        private TaskCompletionSource<bool> _turnPromise;

        public PlayerCharacterController(Character character) : base(character)
        {
        }

        public override Task Act(Combat combat)
        {
            Console.WriteLine($"{Character.Name} starts their turn!");
            var player = Character as PlayerCharacter;
            player.Energy = player.EnergyMax;
            _turnPromise = new TaskCompletionSource<bool>();
            InvokeOnAction();
            return Task.WhenAll(DrawCards(player), _turnPromise.Task);
        }

        private static async Task DrawCards(PlayerCharacter player)
        {
            await player.Draw(player.DrawLimit);
        }

        public async Task EndTurn()
        {
            Console.WriteLine($"{Character.Name} ends their turn!");
            var player = Character as PlayerCharacter;
            await player.Hand.Sequentially(card => player.Discard(card));
            _turnPromise?.SetResult(true);
        }
    }
}
