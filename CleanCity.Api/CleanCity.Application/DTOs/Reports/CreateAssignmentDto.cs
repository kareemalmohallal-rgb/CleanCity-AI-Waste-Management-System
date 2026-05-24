using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Reports
{
    public class CreateAssignmentDto
    {
        public int ReportId { get; set; }
        public int DriverId { get; set; }
    }
}
