using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Devices
{
    public class AnonymousDeviceReadDto
    {
        public int Id { get; set; }
        public string DeviceIdentifier { get; set; } = default!;
        public string FcmToken { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
