using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.GameFlow;

namespace Irrelephant.DnB.Server.Repositories
{
    public class MemoryCombatRepository : ICombatRepository
    {
        private readonly IDictionary<Guid, Combat> _ongoingCombats = new Dictionary<Guid, Combat>();


        public Task<Combat> GetCombat(Guid id)
        {
            return Task.FromResult(_ongoingCombats[id]);
        }

        public Task<Combat> AddCombat(Combat combat)
        {
            _ongoingCombats.Add(combat.CombatId, combat);
            return Task.FromResult(combat);
        }
    }
}
