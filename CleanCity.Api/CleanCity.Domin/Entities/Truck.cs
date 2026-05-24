using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Domain.Entities
{
    public class Truck
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Size { get; set; }
        public bool IsActive { get; set; }=false;
        public int? TruckTypeId { get; set; }
        public TruckType? TruckType { get; set; }

        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }


    }
}
