using CleanCity.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Reports
{
    public class AssignmentReadDto
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int DriverId { get; set; }
        public AssignmentStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? ActionAt { get; set; }
    }
}
