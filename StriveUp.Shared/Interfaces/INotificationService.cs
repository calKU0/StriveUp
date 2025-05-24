using StriveUp.Shared.DTOs;

namespace StriveUp.Shared.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetMyNotificationsAsync();

        Task MarkAsReadAsync(int id);

        Task MarkAllAsReadAsync();
    }
}