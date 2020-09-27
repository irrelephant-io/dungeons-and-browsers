using System.Threading.Tasks;

namespace Irrelephant.DnB.DataTransfer.Services
{
    public interface IApiTokenProvider
    {
        Task<string> GetToken();
    }
}
