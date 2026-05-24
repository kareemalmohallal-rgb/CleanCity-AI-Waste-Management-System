using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.Files
{
    public class UpdateFileDto
    {
        public string FileName { get; set; } = default!;
        public string FilePath { get; set; } = default!;
    }
}
