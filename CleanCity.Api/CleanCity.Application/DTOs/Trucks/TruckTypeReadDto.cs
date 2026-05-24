using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Trucks
{
    public class TruckTypeReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
