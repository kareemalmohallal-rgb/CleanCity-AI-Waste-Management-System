using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Files
{
     public class FileReadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = default!;
        public string FilePath { get; set; } = default!;
        public int? ReportId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
