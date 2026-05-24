using CleanCity.Application.DTOs.AreaListItem;
using CleanCity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IAreaService
    {
        Task<int> CreateAsync(Area area, CancellationToken ct = default);
        Task UpdateAsync(Area area, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<Area?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<Area>> GetAllAsync(CancellationToken ct = default);
        Task<List<AreaListItemDto>> GetAllForIndexAsync(CancellationToken ct = default);
    }
}
