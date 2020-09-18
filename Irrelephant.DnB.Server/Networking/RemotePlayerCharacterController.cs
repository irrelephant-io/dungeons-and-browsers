using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Networking
{
    public class RemotePlayerCharacterController : PlayerCharacterController
    {
        public RemotePlayerCharacter RemoteCharacter => Character as RemotePlayerCharacter;

        public RemotePlayerCharacterController(Character character) : base(character)
        {
        }

        public async override Task Act(Combat combat)
        {
            var sendMyTurnTask = RemoteCharacter.HubClient.SendAsync("MyTurn");
            var sendMyUpdateTask =
                RemoteCharacter.HubClient.SendAsync("CharacterUpdated", RemoteCharacter.GetCharacterSnapshot(sendDeck: false));
            await Task.WhenAll(sendMyTurnTask, sendMyUpdateTask);
            await base.Act(combat);
        }
    }
}
