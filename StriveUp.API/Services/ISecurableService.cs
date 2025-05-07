using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.API.Services
{
    public interface ISecurableService
    {
        Task<string> GetMapboxTokenAsync();
    }
}
