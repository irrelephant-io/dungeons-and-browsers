using System.Collections.Generic;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data.Effects;

namespace Irrelephant.DnB.Core.Data
{
    public interface ITargetProvider
    {
        Task<IEnumerable<Character>> PickTarget(Effect e);
    }
}
