using System.Threading.Tasks;

namespace Irrelephant.DnB.Client.Clients
{
    public interface IAuthHttpClient
    {
        public Task<string> LogInAsync(string idToken);
    }
}
