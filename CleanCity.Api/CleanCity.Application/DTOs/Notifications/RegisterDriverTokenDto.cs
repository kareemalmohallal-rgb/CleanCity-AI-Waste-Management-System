using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Notifications
{
    public class RegisterDriverTokenDto
    {
        public int DriverId { get; set; }
        public string FcmToken { get; set; } = string.Empty;
    }
}
