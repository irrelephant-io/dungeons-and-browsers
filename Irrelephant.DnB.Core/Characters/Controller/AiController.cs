using System.Threading.Tasks;

namespace Irrelephant.DnB.Core.Characters.Controller
{
    public class AiController : CharacterController
    {
        public AiController(Character character) : base(character)
        {
        }

        public override Task Act()
        {
            return Task.CompletedTask;
        }
    }
}
