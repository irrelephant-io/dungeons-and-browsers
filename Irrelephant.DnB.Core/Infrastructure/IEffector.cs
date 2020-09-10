using System.Threading.Tasks;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;

namespace Irrelephant.DnB.Core.Infrastructure
{
    public interface IEffector
    {
        Task CreateEffect(EffectType type, Character target);

        Task PostPrompt(string prompt, int timeMs);

        Task CreateDelay(int timeMs);
    }
}
