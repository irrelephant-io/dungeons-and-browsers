using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.GameFlow;

namespace Irrelephant.DnB.Server.Repositories
{
    public class MemoryCombatRepository : ICombatRepository
    {
        private readonly IDictionary<Guid, Combat> _ongoingCombats = new Dictionary<Guid, Combat>();


        public Task<IEnumerable<Combat>> ListCombats()
        {
            return Task.FromResult(_ongoingCombats.Values.AsEnumerable());
        }

        public Task<Combat> GetCombat(Guid id)
        {
            if (_ongoingCombats.ContainsKey(id))
            {
                return Task.FromResult(_ongoingCombats[id]);
            }
            else
            {
                return Task.FromResult<Combat>(null);
            }
        }

        public Task<Combat> AddCombat(Combat combat)
        {
            _ongoingCombats.Add(combat.CombatId, combat);
            return Task.FromResult(combat);
        }

        public Task RemoveCombat(Combat combat)
        {
            _ongoingCombats.Remove(combat.CombatId);
            return Task.CompletedTask;
        }
    }
}
