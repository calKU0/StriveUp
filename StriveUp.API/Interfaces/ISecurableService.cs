namespace StriveUp.API.Interfaces
{
    public interface ISecurableService
    {
        Task<string> GetMapboxTokenAsync();
    }
}