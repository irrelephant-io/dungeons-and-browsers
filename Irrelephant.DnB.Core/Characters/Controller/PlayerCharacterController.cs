using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.GameFlow;

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
            _turnPromise = new TaskCompletionSource<bool>();
            return _turnPromise.Task;
        }

        public void EndTurn()
        {
            Console.WriteLine($"{Character.Name} ends their turn!");
            _turnPromise?.SetResult(true);
        }
    }
}
