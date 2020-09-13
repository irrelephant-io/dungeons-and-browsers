using System.Threading.Tasks;
using DeepEqual.Syntax;
using Irrelephant.DnB.Client.Tests;
using Irrelephant.DnB.Core.Networking;
using Irrelephant.DnB.Server.SampleData;
using Irrelephant.DnB.Server.Tests.Fixtures;
using Irrelephant.DnB.Server.Tests.Infrastructure;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Irrelephant.DnB.Server.Tests
{
    [Collection(nameof(ServerTestsCollection))]
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
            await AssertExtensions.Eventually(() => {
                receivedSnapshot.ShouldDeepEqual(CombatFactory.BuildCombat().GetSnapshot());
            });
        }
    }
}
