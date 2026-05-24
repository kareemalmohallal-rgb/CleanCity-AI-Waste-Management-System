using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanCity.Domain.Enum;

namespace CleanCity.Domain.Entities
{
    public class ReportAssignment
    {
        public int Id { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; } = default!;

        public int DriverId { get; set; }
        public Driver Driver { get; set; } = default!;

        public AssignmentStatus Status { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime AssignedAt { get; set; }
        public DateTime? ActionAt { get; set; }
    }

}
