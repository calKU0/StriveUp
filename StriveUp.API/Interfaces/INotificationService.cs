using StriveUp.Shared.DTOs;

namespace StriveUp.API.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(CreateNotificationDto notificationDto);
    }
}