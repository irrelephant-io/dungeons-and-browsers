using Irrelephant.DnB.Server.Tests.Fixtures;
using Xunit;

namespace Irrelephant.DnB.Server.Tests.Infrastructure
{

    [CollectionDefinition(nameof(ServerTestsCollection), DisableParallelization = true)]
    public class ServerTestsCollection : ICollectionFixture<ServerFixture>
    {
    }
}
