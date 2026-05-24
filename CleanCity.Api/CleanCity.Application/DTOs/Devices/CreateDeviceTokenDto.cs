using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Devices
{
    public class CreateDeviceTokenDto
    {
        public string UserId { get; set; } = default!;
        public string Token { get; set; } = default!;
        public string DeviceType { get; set; } = default!;
        public bool IsActive { get; set; } = true;
    }
}
