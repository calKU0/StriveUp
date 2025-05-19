using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface ITokenStorageService
    {
        Task StoreToken(string token);
        Task<string?> GetToken();
        Task ClearToken();
        Task StoreRefreshToken(string token);
        Task<string?> GetRefreshToken();
        Task ClearRefreshToken();
    }
}
