using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Drivers
{
    public class UpdateDriverDto
    {
        public string FullName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? LicenseNumber { get; set; }
        public bool IsActive { get; set; }
        public int? AreaId { get; set; }
    }
}
