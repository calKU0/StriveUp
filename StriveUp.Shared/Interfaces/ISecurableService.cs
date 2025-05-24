namespace StriveUp.Shared.Interfaces
{
    public interface ISecurableService
    {
        Task<string> GetMapboxTokenAsync();
    }
}