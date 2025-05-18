using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface ISynchroService
    {
        Task<List<UserSynchroDto>> GetAvailableProvidersAsync();
        Task<List<UserSynchroDto>> GetUserSynchrosAsync();
        Task<HttpResponseMessage> UpdateUserSynchroAsync(int id, UpdateUserSynchroDto dto);
        Task<HttpResponseMessage> DeleteUserSynchroAsync(int id);
        string GetOAuthUrl(string provider);
        Task<HttpResponseMessage> ExchangeCodeAsync(string code, string state);
    }
}
