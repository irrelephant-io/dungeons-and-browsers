using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.DataTransfer.Models;
using Irrelephant.DnB.Server.Tests.Fixtures;
using Irrelephant.DnB.Server.Tests.Infrastructure;
using Irrelephant.DnB.Tests.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Irrelephant.DnB.Server.Tests
{
    [Collection(nameof(ServerTestsCollection))]
    [Trait("Category", "Integration")]
    public class CombatHubTests
    {
        private readonly ServerFixture _fixture;

        public CombatHubTests(ServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ShouldBeAble_ToSendJoinRequest()
        {
            await _fixture.CombatConnection.SendAsync("JoinCombat");
        }

        [Fact]
        public async Task ShouldEventuallyGetCombatSnapshot_AfterSendingJoinRequest()
        {
            var combatId = Guid.NewGuid();
            CombatSnapshot receivedSnapshot = null;
            _fixture.CombatConnection.On<CombatSnapshot>("Joined", snap => {
                receivedSnapshot = snap; 
            });
            await _fixture.CombatConnection.SendAsync("JoinCombat", combatId);
            await AssertUtilities.Eventually(() => {
                Assert.Equal(1, receivedSnapshot.Turn);
                Assert.NotEmpty(receivedSnapshot.Attackers);
                Assert.NotEmpty(receivedSnapshot.Defenders);
                Assert.Empty(receivedSnapshot.PendingAttackers);
                Assert.Empty(receivedSnapshot.PendingDefenders);
                Assert.Contains(receivedSnapshot.Attackers, attacker => attacker.Id == receivedSnapshot.ActiveCharacterId);
                var myDeck = receivedSnapshot.Attackers.Single(it => it.Id == receivedSnapshot.ActiveCharacterId).Deck;
                Assert.NotEmpty(myDeck.DrawPile);
                Assert.Empty(myDeck.Hand);
                Assert.Empty(myDeck.DiscardPile);
            });
        }
    }
}
