using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Irrelephant.DnB.Tests.Utilities
{
    public class AssertionUtilities
    {
        public static void OnlyContains<TItem>(IEnumerable<TItem> targetCollection, IEnumerable<TItem> targets)
        {
            Assert.Empty(targetCollection.Except(targets));
        }
    }
}
