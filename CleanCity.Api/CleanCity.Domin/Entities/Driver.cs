using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Domain.Entities
{
    public class Driver
    {
        public int Id { get; set; }

 

        public string FullName { get; set; }
        public string? LicenseNumber { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        public string? FcmToken { get; set; }
        public int? TruckId { get; set; }
        public Truck? Truck { get; set; }
        public ICollection<ReportAssignment>? Assignments { get; set; } = new List<ReportAssignment>();
        public ICollection<Report>? CurrentReports { get; set; } = new List<Report>(); // اختياري

    }
}
