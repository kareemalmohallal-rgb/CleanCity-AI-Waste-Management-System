using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Devices
{
    public class UpdateDeviceTokenDto
    {
        public bool IsActive { get; set; }
        public string DeviceType { get; set; } = default!;
    }
}
