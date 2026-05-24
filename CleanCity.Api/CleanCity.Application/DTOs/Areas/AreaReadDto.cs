using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Areas
{
    public class AreaReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public double CenterLatitude { get; set; }
        public double CenterLongitude { get; set; }
        public double RadiusInMeters { get; set; }
    }
}
