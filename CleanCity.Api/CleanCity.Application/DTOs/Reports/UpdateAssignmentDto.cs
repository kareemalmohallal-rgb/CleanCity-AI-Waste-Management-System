using CleanCity.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Reports
{
    public class UpdateAssignmentDto
    {
        public AssignmentStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ActionAt { get; set; }
    }
}
