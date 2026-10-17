using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IPurchaseOrderRepository: IGenericRepository<PurchaseOrder>
    {
        Task<PurchaseOrder?> GetByIdAsync(int id, bool includeItems = false, CancellationToken ct = default);
        Task<IReadOnlyList<PurchaseOrder>> GetAllAsync(bool includeItems = false, CancellationToken ct = default);
        Task<IReadOnlyList<PurchaseOrder>> GetBySupplierIdAsync(int supplierId, CancellationToken ct = default);
        Task<IReadOnlyList<PurchaseOrder>> GetByDestinationWarehouseIdAsync(int warehouseId, CancellationToken ct = default);
        Task<IReadOnlyList<PurchaseOrder>> GetByStatusAsync(string status, CancellationToken ct = default);
        Task<IReadOnlyList<PurchaseOrder>> GetByOrderDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);


    }
}
