using GymSystem.BLL.Dtos;
using GymSystem.BLL.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Interfaces.Business
{
    public interface INotificationRepo
    {
        Task<IEnumerable<NotificationDto>> GetNotifications();
        Task<ApiResponse> AddNotification(NotificationDto notificationDto);
        Task<ApiResponse> DeleteNotification(int notificationId);
    }
}
