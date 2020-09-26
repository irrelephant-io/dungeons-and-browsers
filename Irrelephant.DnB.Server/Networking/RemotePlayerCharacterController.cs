using System.Security.Principal;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Characters.Controller;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.GameFlow;
using Irrelephant.DnB.DataTransfer.Models;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Networking
{
    public class RemotePlayerCharacterController : PlayerCharacterController
    {
        public RemotePlayerCharacter RemoteCharacter => Character as RemotePlayerCharacter;

        public IIdentity ControllingIdentity { get; set; }

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

        public async override Task JoinPendingCombat(JoinedSide side, int position)
        {
            await base.JoinPendingCombat(side, position);
            var message = new JoinFightMessage {
                Character = Character.GetCharacterSnapshot(), Side = side, Position = position
            };
            await RemoteCharacter.BroadcastHubClient.SendAsync("PendingCombat", message);
        }

        public async override Task JoinCombat(JoinedSide side, int position)
        {
            await base.JoinCombat(side, position);
            var message = new JoinFightMessage {
                Character = Character.GetCharacterSnapshot(), Side = side, Position = position
            };
            await RemoteCharacter.BroadcastHubClient.SendAsync("CharacterJoined", message);
        }
    }
}
