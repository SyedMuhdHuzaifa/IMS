using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, bool includeInactive = false, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> SearchByNameAsync(string term, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<bool> SkuExistsAsync(string sku, int? excludeId = null, CancellationToken ct = default);
        Task DeactivateAsync(int id, CancellationToken ct = default);
    }
}
