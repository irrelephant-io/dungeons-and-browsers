using System.Threading.Tasks;

namespace Irrelephant.DnB.Server.Authentication.Services
{
    public interface IAuthenticationService
    { 
        Task<string> Authenticate(string idToken);
    }
}
