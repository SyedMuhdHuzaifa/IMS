using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(AppDbContext context) : base(context) { }
       public override async Task<IReadOnlyList<Warehouse>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(w => w.Code)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Warehouse>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(w => w.IsActive)
                .OrderBy(w => w.Code)
                .ToListAsync(ct);
        }
       public Task<Warehouse?> GetByCodeAsync(string code, CancellationToken ct = default)
        {
            var normalized = code.Trim();
            return _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Code == normalized, ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => _dbSet.AnyAsync(w => w.Id == id, ct);
        public Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken ct = default)
        {
            var normalized = code.Trim();
            return _dbSet.AnyAsync(w =>
                w.Code == normalized &&
                (!excludeId.HasValue || w.Id != excludeId.Value), ct);
        }

       public async Task DeactivateAsync(int id, CancellationToken ct = default)
        {
            var warehouse = await _dbSet.FindAsync(new object[] { id }, ct)
                ?? throw new KeyNotFoundException($"Warehouse '{id}' was not found.");

            warehouse.IsActive = false;
        }
    }
}