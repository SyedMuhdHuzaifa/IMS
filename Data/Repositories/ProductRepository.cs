
using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ProductRepository
        : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<Product?> GetBySkuAsync(
            string sku,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return null;

            sku = sku.Trim();

            return await _context.Products
                .FirstOrDefaultAsync(
                    product => product.SKU == sku,
                    ct);
        }

        public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(
            int categoryId,
            bool includeInactive = false,
            CancellationToken ct = default)
        {
            IQueryable<Product> query = _context.Products
                .Where(product => product.CategoryId == categoryId);

            if (!includeInactive)
            {
                query = query.Where(product => product.IsActive);
            }

            return await query
                .OrderBy(product => product.Name)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Product>> SearchByNameAsync(
            string term,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Array.Empty<Product>();

            term = term.Trim();

            return await _context.Products
                .Where(product => product.Name.Contains(term))
                .OrderBy(product => product.Name)
                .ToListAsync(ct);
        }

        public async Task<bool> ExistsAsync(
            int id,
            CancellationToken ct = default)
        {
            return await _context.Products
                .AnyAsync(product => product.Id == id, ct);
        }

        public async Task<bool> SkuExistsAsync(
            string sku,
            int? excludeId = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return false;

            sku = sku.Trim();

            IQueryable<Product> query = _context.Products
                .Where(product => product.SKU == sku);

            if (excludeId.HasValue)
            {
                query = query.Where(
                    product => product.Id != excludeId.Value);
            }

            return await query.AnyAsync(ct);
        }

        public async Task DeactivateAsync(
            int id,
            CancellationToken ct = default)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(
                    product => product.Id == id,
                    ct);

            if (product is null)
            {
                throw new KeyNotFoundException(
                    $"Product with ID '{id}' was not found.");
            }

            product.IsActive = false;
        }
    }
}
