 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.AreaListItem
{
    public sealed class AreaListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int ReportsCount { get; set; }
        public string? ResponsibleDriverName { get; set; }
    }
}
