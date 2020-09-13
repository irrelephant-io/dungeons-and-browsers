using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;

namespace Irrelephant.DnB.Client.Networking
{
    public class ClientCard : Card
    {
        public string RemoteText { get; set; }

        public override string Text => RemoteText;

        public override Task Play(PlayerCharacter player, ITargetProvider targetProvider)
        {
            return Task.CompletedTask;
        }
    }
}
