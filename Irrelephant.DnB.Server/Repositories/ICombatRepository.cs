using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.GameFlow;

namespace Irrelephant.DnB.Server.Repositories
{
    public interface ICombatRepository
    {
        Task<Combat> GetCombat(Guid id);

        Task<Combat> AddCombat(Combat combat);

        Task RemoveCombat(Combat combat);
    }
}
