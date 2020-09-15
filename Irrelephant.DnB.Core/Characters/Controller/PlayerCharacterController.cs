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
            _turnPromise = new TaskCompletionSource<bool>();
            InvokeOnAction();
            return _turnPromise.Task;
        }

        public Task EndTurn()
        {
            _turnPromise?.SetResult(true);
            return Task.CompletedTask;
        }
    }
}
