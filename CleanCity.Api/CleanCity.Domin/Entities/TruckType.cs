using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Domain.Entities
{
    public class TruckType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        ICollection<Truck>? Trucks { get; set; } = new List<Truck>();
    }
}
