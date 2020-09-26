using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Server.Hubs;
using Irrelephant.DnB.Server.SampleData;
using Microsoft.AspNetCore.SignalR;

namespace Irrelephant.DnB.Server.Networking
{
    public class RemotePlayerCharacter : PlayerCharacter
    {
        private readonly IHubContext<CombatHub> _combatHubContext;

        public string ConnectionId { get; set; }

        public IClientProxy HubClient => _combatHubContext.Clients.Client(ConnectionId);

        public IClientProxy BroadcastHubClient => _combatHubContext.Clients.All;

        public RemotePlayerCharacter(IHubContext<CombatHub> combatHubContext)
        {
            _combatHubContext = combatHubContext;
        }

        public async override Task Discard(Card card)
        {
            await base.Discard(card);
            await HubClient.SendAsync("DiscardCard", card.Id);
        }

        public async override Task<Card> Draw()
        {
            var drawnCard = await base.Draw();
            await HubClient.SendAsync("DrawCard", drawnCard.Id);
            return drawnCard;
        }

        protected override Task ReshuffleDiscard()
        {
            return Task.WhenAll(base.ReshuffleDiscard(), HubClient.SendAsync("ReshuffleDiscardPile"));
        }

        public async override Task DealDamage(int amount, bool ignoreArmor = false)
        {
            await base.DealDamage(amount, ignoreArmor);
            await HubClient.SendAsync("CharacterUpdated", CombatFactory.GetCharacterSnapshot(this, sendDeck: false));
        }
    }
}
