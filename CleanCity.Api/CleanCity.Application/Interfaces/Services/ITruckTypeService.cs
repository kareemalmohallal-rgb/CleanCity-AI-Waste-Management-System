using CleanCity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.Interfaces.Services
{
    public interface ITruckTypeService
    {
        Task<int> CreateAsync(TruckType type, CancellationToken ct = default);
        Task UpdateAsync(TruckType type, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<TruckType?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<TruckType>> GetAllAsync(CancellationToken ct = default);
    }
}
