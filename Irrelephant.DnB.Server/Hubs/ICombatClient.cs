using System;
using System.Threading.Tasks;
using Irrelephant.DnB.DataTransfer.Models;

namespace Irrelephant.DnB.Server.Hubs
{
    public interface ICombatClient
    {
        Task Joined(CombatSnapshot snapshot);

        Task MyTurn();

        Task CardPlayed(Guid id);
    }
}
