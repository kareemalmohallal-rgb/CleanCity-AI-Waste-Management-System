using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Devices
{
    public class CreateAnonymousDeviceDto
    {
        public string DeviceIdentifier { get; set; } = default!;
        public string FcmToken { get; set; } = default!;
    }
}
