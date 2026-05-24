using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.AwarenessContents
{
    public class CreateAwarenessContentDto
    {
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string? ImageUrl { get; set; }
    }
}
