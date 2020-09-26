using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Irrelephant.DnB.Client.Infrastructure
{
    public interface IApiTokenProvider
    {
        public Task<string> GetToken();
    }
}
