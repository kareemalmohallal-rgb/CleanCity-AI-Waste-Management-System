using CleanCity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.Interfaces.Services
{
    public interface IDriverService
    {
        Task<int> CreateAsync(Driver driver, CancellationToken ct = default);
        Task UpdateAsync(Driver driver, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<Driver?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<Driver>> GetAllAsync(CancellationToken ct = default);
    }
}
