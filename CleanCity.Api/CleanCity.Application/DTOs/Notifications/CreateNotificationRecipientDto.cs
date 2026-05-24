using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Notifications
{
    public class CreateNotificationRecipientDto
    {
        public int NotificationId { get; set; }

        public int? AnonymousDeviceId { get; set; }
        public int? DriverId { get; set; }
        public int? DeviceTokenId { get; set; }
        public string? UserId { get; set; }
    }
}
