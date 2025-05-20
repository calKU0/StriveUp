using StriveUp.Shared.Interfaces;

public class NotificationStateService : INotificationStateService, IAsyncDisposable
{
    private readonly INotificationService _notificationService;
    private Timer? _timer;
    private int _unreadCount;

    public event Action? OnChange;

    public int UnreadCount
    {
        get => _unreadCount;
        set
        {
            if (_unreadCount != value)
            {
                _unreadCount = value;
                NotifyStateChanged();
            }
        }
    }

    public NotificationStateService(INotificationService notificationService)
    {
        _notificationService = notificationService;
        _timer = new Timer(async _ => await RefreshUnreadCountAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }

    public async Task RefreshUnreadCountAsync()
    {
        try
        {
            var notifications = await _notificationService.GetMyNotificationsAsync();
            UnreadCount = notifications.Count(n => !n.IsRead);
        }
        catch
        {
            // Handle/log as needed
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        if (_timer is not null)
        {
            await _timer.DisposeAsync();
            _timer = null;
        }
    }
}