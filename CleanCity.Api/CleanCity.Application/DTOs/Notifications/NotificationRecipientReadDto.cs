using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Notifications
{
    public class NotificationRecipientReadDto
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public int? AnonymousDeviceId { get; set; }
        public int? DriverId { get; set; }
        public int? DeviceTokenId { get; set; }
        public string? UserId { get; set; }
    }
}
