using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class WarehouseStockRepository : GenericRepository<WarehouseStock>, IWarehouseStockRepository
    {

       public WarehouseStockRepository(AppDbContext context) : base(context) { }
       public Task<WarehouseStock?> GetByWarehouseAndProductAsync(int warehouseId, int productId, CancellationToken ct = default)
            => _dbSet.FirstOrDefaultAsync(ws => ws.WarehouseId == warehouseId && ws.ProductId == productId, ct);
       public async Task<IReadOnlyList<WarehouseStock>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default)
       {
            return await _dbSet
                .AsNoTracking()
                .Include(ws => ws.Product)
                .Where(ws => ws.WarehouseId == warehouseId)
                .OrderBy(ws => ws.ProductId)
                .ToListAsync(ct);
       }
        public async Task<IReadOnlyList<WarehouseStock>> GetByProductAsync(int productId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ws => ws.Warehouse)
                .Where(ws => ws.ProductId == productId)
                .OrderBy(ws => ws.WarehouseId)
                .ToListAsync(ct);
        }
        public Task<int> GetTotalOnHandAsync(int productId, CancellationToken ct = default)
            => _dbSet
                .Where(ws => ws.ProductId == productId)
                .SumAsync(ws => ws.QuantityOnHand, ct);
        public async Task<IReadOnlyList<(int ProductId, string ProductName, string SKU, int ReorderLevel, int TotalOnHand)>> GetLowStockAsync(CancellationToken ct = default)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive &&
                            (p.WarehouseStocks.Sum(ws => (int?)ws.QuantityOnHand) ?? 0) <= p.ReorderLevel)
                .OrderBy(p => p.Name)
                .Select(p => new ValueTuple<int, string, string, int, int>(
                    p.Id,
                    p.Name,
                    p.SKU,
                    p.ReorderLevel,
                    p.WarehouseStocks.Sum(ws => (int?)ws.QuantityOnHand) ?? 0))
                .ToListAsync(ct);
        }
    }
}