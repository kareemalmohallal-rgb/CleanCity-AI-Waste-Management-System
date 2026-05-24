using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Areas
{
    public class CreateAreaDto
    {
        public string Name { get; set; } = default!;
        public double MinLat { get; set; }
        public double MaxLat { get; set; }
        public double MinLng { get; set; }
        public double MaxLng { get; set; }
    }
}
