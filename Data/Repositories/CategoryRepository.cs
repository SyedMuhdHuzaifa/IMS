using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<Category?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            name = name.Trim();

            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name, ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => _dbSet.AnyAsync(c => c.Id == id, ct);

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            name = name.Trim();

            return await _dbSet.AnyAsync(c =>
                c.Name == name &&
                (!excludeId.HasValue || c.Id != excludeId.Value), ct);
        }

        // One query: the count is done in SQL, and the Products collection is never loaded.
        public async Task<IReadOnlyList<Category>> GetAllWithProductCountsAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(ct);
        }

        public Task<int> GetProductCountAsync(int categoryId, CancellationToken ct = default)
            => _context.Products.CountAsync(p => p.CategoryId == categoryId, ct);
    }
}