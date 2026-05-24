using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Trucks
{
    public class TruckReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Number { get; set; } = default!;
        public string Size { get; set; } = default!;
        public bool IsActive { get; set; }
        public int? TruckTypeId { get; set; }
        public int? DriverId { get; set; }
    }

}
