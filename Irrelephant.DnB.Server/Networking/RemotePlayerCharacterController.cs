using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Networking
{
    public class RemotePlayerCharacterController : PlayerCharacterController
    {
        public RemotePlayerCharacter RemoteCharacter => Character as RemotePlayerCharacter;

        public RemotePlayerCharacterController(Character character) : base(character)
        {
        }

        public override Task Act(Combat combat)
        {
            var sendMyTurnTask = RemoteCharacter.HubClient.SendAsync("MyTurn");
            return Task.WhenAll(sendMyTurnTask, base.Act(combat));
        }
    }
}
