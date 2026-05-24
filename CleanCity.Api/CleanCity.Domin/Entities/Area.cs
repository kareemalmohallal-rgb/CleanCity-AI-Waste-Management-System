using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Domain.Entities
{
    public class Area
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double MinLat { get; set; }
        public double MaxLat { get; set; }
        public double MinLng { get; set; }
        public double MaxLng { get; set; }

        public ICollection<Driver>? Drivers { get; set; } = new List<Driver>();
        public ICollection<Report>? Reports { get; set; } = new List<Report>();

    }
}
