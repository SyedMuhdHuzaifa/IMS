using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public Task<Product?> GetByIdWithCategoryAsync(int id, CancellationToken ct = default)
            => _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<IReadOnlyList<Product>> GetAllWithCategoryAsync(bool includeInactive = false, CancellationToken ct = default)
        {
            IQueryable<Product> query = _dbSet
                .AsNoTracking()
                .Include(p => p.Category);

            if (!includeInactive)
                query = query.Where(p => p.IsActive);

            return await query
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
        }

        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return null;

            sku = sku.Trim();

            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.SKU == sku, ct);
        }

        public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, bool includeInactive = false, CancellationToken ct = default)
        {
            IQueryable<Product> query = _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId);

            if (!includeInactive)
                query = query.Where(p => p.IsActive);

            return await query
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Product>> SearchByNameAsync(string term, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Array.Empty<Product>();

            term = term.Trim();

            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(term))
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => _dbSet.AnyAsync(p => p.Id == id, ct);

        public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return false;

            sku = sku.Trim();

            return await _dbSet.AnyAsync(p =>
                p.SKU == sku &&
                (!excludeId.HasValue || p.Id != excludeId.Value), ct);
        }
    }
}