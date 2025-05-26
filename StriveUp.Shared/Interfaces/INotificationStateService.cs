namespace StriveUp.Shared.Interfaces
{
    public interface INotificationStateService
    {
        int UnreadCount { get; set; }

        event Action? OnChange;

        Task RefreshUnreadCountAsync();
    }
}