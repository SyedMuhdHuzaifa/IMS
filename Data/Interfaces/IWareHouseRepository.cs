using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IWarehouseRepository: IGenericRepository<Warehouse>
    {
        Task<Warehouse?> GetByCodeAsync(string code, CancellationToken ct = default);
        Task<IReadOnlyList<Warehouse>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken ct = default);
        Task DeactivateAsync(int id, CancellationToken ct = default);
    }
}
