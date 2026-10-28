using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext context) : base(context) { }
       public override async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync(ct);
        }
       public Task<Supplier?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var normalized = email.Trim();
            return _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == normalized, ct);
        }
       public async Task<IReadOnlyList<Supplier>> SearchByNameAsync(string term, CancellationToken ct = default)
        {
            var normalized = term?.Trim();

            if (string.IsNullOrEmpty(normalized))
                return Array.Empty<Supplier>();

            return await _dbSet
                .AsNoTracking()
                .Where(s => s.Name.Contains(normalized))
                .OrderBy(s => s.Name)
                .ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => _dbSet.AnyAsync(s => s.Id == id, ct);

        public Task<int> GetPurchaseOrderCountAsync(int supplierId, CancellationToken ct = default)
            => _context.PurchaseOrders.CountAsync(po => po.SupplierId == supplierId, ct);
    }
}