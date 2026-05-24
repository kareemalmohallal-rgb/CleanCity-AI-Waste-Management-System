using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Domain.Entities
{
    public class FileEntity
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ReportId { get; set; }
        public Report? Report { get; set; }
    }
}
