
using IMS.Entities;

namespace Data.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(
        string name,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(
        int id,
        CancellationToken ct = default);
}
