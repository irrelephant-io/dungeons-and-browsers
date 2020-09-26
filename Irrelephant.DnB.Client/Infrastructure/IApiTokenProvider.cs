using System.Threading.Tasks;

namespace Irrelephant.DnB.Client.Infrastructure
{
    public interface IApiTokenProvider
    {
        public Task<string> GetToken();
    }
}
