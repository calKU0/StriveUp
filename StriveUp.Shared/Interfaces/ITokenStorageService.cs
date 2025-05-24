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