using CleanCity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface IDriverRepository
    {
        // جلب سائق بالمعرّف
        Task<Driver?> GetByIdAsync(int id, CancellationToken ct = default);

        // جلب كل السائقين
        Task<List<Driver>> GetAllAsync(CancellationToken ct = default);

        // إضافة سائق
        Task AddAsync(Driver driver, CancellationToken ct = default);

        // تعديل سائق
        void Update(Driver driver);

        // إزالة سائق
        void Remove(Driver driver);

        // هل المعرّف موجود؟
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

        // هل السائق نشط؟
        Task<bool> IsActiveAsync(int id, CancellationToken ct = default);
        Task<Driver?> GetLeastLoadedActiveByAreaAsync(int areaId, CancellationToken ct);
    }
}
