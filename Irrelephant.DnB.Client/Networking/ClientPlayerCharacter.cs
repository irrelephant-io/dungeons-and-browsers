using Irrelephant.DnB.Core.Characters;

namespace Irrelephant.DnB.Client.Networking
{
    public class ClientPlayerCharacter : PlayerCharacter
    {
        public virtual ClientCombat ClientCombat { get; set; }
    }
}
