using System.Threading.Tasks;

namespace Irrelephant.DnB.DataTransfer.Clients
{
    public interface IAuthHttpClient
    {
        Task<string> LogInAsync(string idToken);
    }
}
