using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Reports
{
    public class CreateReportDto
    {
        public string ImageUrl { get; set; } = "_";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
        public int AnonymousDeviceId { get; set; }
        public Stream? ImageStream { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageContentType { get; set; }
    }
}
