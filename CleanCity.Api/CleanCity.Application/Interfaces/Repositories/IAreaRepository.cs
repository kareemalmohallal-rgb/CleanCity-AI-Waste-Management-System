using CleanCity.Application.DTOs.AreaListItem;
using CleanCity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.Interfaces.Repositories
{
    public interface IAreaRepository
    {
        // جلب عنصر واحد بالمعرّف
        Task<Area?> GetByIdAsync(int id, CancellationToken ct = default);

        // جلب كل المناطق
        Task<List<Area>> GetAllAsync(CancellationToken ct = default);
        Task<List<AreaListItemDto>> GetAllForIndexAsync(CancellationToken ct = default);
        // إضافة منطقة جديدة
        Task AddAsync(Area area, CancellationToken ct = default);

        // تعديل منطقة
        void Update(Area area);

        // حذف منطقة
        void Remove(Area area);

        // التحقق من وجود منطقة بالمعرّف
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<Area?> FindDistrictAsync(double Latitude, double Longitude, CancellationToken ct);
    }
}
