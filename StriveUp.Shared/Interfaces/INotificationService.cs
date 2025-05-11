using StriveUp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(CreateNotificationDto notification);
        Task<List<NotificationDto>> GetMyNotificationsAsync();
        Task MarkAsReadAsync(int id);
        Task MarkAllAsReadAsync();
    }
}
