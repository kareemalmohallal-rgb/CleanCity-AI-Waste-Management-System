using CleanCity.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Reports
{
    public class ReportReadDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = default!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }

        public ReportStatus Status { get; set; }
        public int AnonymousDeviceId { get; set; }
        public int? AreaId { get; set; }

        public int? CurrentDriverId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
