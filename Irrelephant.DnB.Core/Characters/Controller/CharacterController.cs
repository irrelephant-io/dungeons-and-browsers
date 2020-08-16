
using System.Threading.Tasks;

namespace Irrelephant.DnB.Core.Characters.Controller
{
    public abstract class CharacterController
    {
        public Character Character { get; }

        protected CharacterController(Character character)
        {
            Character = character;
        }

        public abstract Task Act();

    }
}
