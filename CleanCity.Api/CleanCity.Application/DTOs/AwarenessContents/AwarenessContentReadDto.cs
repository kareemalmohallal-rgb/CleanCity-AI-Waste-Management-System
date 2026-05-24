using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.AwarenessContents
{
    public class AwarenessContentReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
