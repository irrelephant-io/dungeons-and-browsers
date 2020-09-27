using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.DataTransfer.Models;
using Irrelephant.DnB.Server.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Irrelephant.DnB.Server.Controllers
{
    public class LobbyController : ApiControllerBase
    {
        private readonly ICombatRepository _combatRepository;

        public LobbyController(ICombatRepository combatRepository)
        {
            _combatRepository = combatRepository;
        }

        [HttpGet("combats")]
        [ProducesResponseType(typeof(IEnumerable<CombatInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCombats()
        {
            var combats = await _combatRepository.ListCombats();
            var combatInfos = combats.Select(combat => new CombatInfo {
                CombatId = combat.CombatId,
                Round = combat.Round,
                Attackers = combat.Attackers.Union(combat.PendingAttackers.Select(pending => pending.cc)).Select(cc => cc.Character.Name),
                Defenders = combat.Defenders.Union(combat.PendingDefenders.Select(pending => pending.cc)).Select(cc => cc.Character.Name)
            });
            return Ok(combatInfos);
        }
    }
}
