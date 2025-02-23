using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Dtos
{
    public class NotificationDto
    {
        public int? NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
    }
}
