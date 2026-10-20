using IMS.Entities;

namespace Data.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Includes Category so the DTO can show CategoryName.
        Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetAllWithCategoryAsync(bool includeInactive = false, CancellationToken ct = default);

        Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, bool includeInactive = false, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> SearchByNameAsync(string term, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

        // Pass excludeId when updating, so a product's own SKU doesn't count as a duplicate.
        Task<bool> SkuExistsAsync(string sku, int? excludeId = null, CancellationToken ct = default);
    }
}