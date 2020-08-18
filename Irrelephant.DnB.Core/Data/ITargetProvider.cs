using System.Collections.Generic;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data.Effects;

namespace Irrelephant.DnB.Core.Data
{
    public interface ITargetProvider
    {
        IEnumerable<Character> PickTarget(Effect e);
    }
}
