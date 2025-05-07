using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface ISecurableService
    {
        Task<string> GetMapboxTokenAsync();
    }
}
