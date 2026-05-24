using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Devices
{
    public class RegisterAnonymousDeviceDto
    {
        public string DeviceIdentifier { get; set; } = string.Empty;
        public string FcmToken { get; set; } = string.Empty;
    }
}
