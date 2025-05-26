using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Interfaces
{
    public interface ISynchroService
    {
        Task<List<SynchroProviderDto>> GetAvailableProvidersAsync();

        Task<List<UserSynchroDto>> GetUserSynchrosAsync();

        Task<HttpResponseMessage> UpdateUserSynchroAsync(int id, UpdateUserSynchroDto dto);

        Task<HttpResponseMessage> DeleteUserSynchroAsync(int id);

        string GetOAuthUrl(string provider);

        Task<HttpResponseMessage> ExchangeCodeAsync(string code, string state);
    }
}