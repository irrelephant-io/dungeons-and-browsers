using System;
using System.Linq;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Irrelephant.DnB.DataTransfer.Models;
using Irrelephant.DnB.Server.SampleData;
using Irrelephant.DnB.Server.Tests.Fixtures;
using Irrelephant.DnB.Server.Tests.Infrastructure;
using Irrelephant.DnB.Tests.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
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
            CombatSnapshot receivedSnapshot = null;
            _fixture.CombatConnection.On<CombatSnapshot>("Joined", snap => {
                receivedSnapshot = snap; 
            });
            await _fixture.CombatConnection.SendAsync("JoinCombat");
            await AssertUtilities.Eventually(() => {
                var expectedSnapshot = CombatFactory.BuildCombat(_fixture.Services).GetSnapshot();
                expectedSnapshot.ActiveCharacterId = receivedSnapshot.ActiveCharacterId;
                receivedSnapshot.Attackers = receivedSnapshot.Attackers
                    .Where(a => a.Id != receivedSnapshot.ActiveCharacterId).ToArray();
                receivedSnapshot.Id = Guid.Empty; // combat id is generated when combat is created, so not checking it
                receivedSnapshot.ShouldDeepEqual(expectedSnapshot);
            });
        }
    }
}
