using IMS.Entities;

namespace Data.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken ct = default);

    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken ct = default);

    Task<IReadOnlyList<Category>> GetAllWithProductCountsAsync(CancellationToken ct = default);

    Task<int> GetProductCountAsync(int categoryId, CancellationToken ct = default);

    Task<IReadOnlyDictionary<int, int>> GetProductCountsAsync(CancellationToken ct = default);
}