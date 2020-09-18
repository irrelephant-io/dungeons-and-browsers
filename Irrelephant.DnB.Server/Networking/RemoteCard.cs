using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;
using Irrelephant.DnB.Server.Hubs;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Networking
{
    public class RemoteCard : Card
    {
        public override Guid Id => _card.Id;

        public override string Name => _card.Name;

        public override string GraphicId => _card.GraphicId;

        public override string Text => _card.Text;

        public override int ActionCost => _card.ActionCost;

        private readonly IHubContext<CombatHub> _combatHubContext;

        private readonly Card _card;

        public override IEnumerable<Effect> Effects => _card.Effects
            .Select(e => new RemoteEffect(_combatHubContext, e))
            .ToArray();

        public RemoteCard(IHubContext<CombatHub> combatHubContext, Card card)
        {
            _combatHubContext = combatHubContext;
            _card = card;
        }

        public async override Task Play(PlayerCharacter player, ITargetProvider targetProvider)
        {
            await base.Play(player, targetProvider);
            var remotePlayer = (RemotePlayerCharacter)player;
            await Task.WhenAll(
                remotePlayer.HubClient.SendAsync("CardPlayed", Id),
                _combatHubContext.Clients.All.SendAsync("CharacterUpdated", remotePlayer.GetCharacterSnapshot())
            );
        }
    }
}
