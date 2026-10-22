using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface ISupplierRepository: IGenericRepository<Supplier>
    {
        Task<Supplier?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IReadOnlyList<Supplier>> SearchByNameAsync(string term, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<int> GetPurchaseOrderCountAsync(int supplierId, CancellationToken ct = default);
    }
}
