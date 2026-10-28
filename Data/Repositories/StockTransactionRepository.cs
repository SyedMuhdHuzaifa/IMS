using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class StockTransactionRepository : GenericRepository<StockTransaction>, IStockTransactionRepository
    {
        public StockTransactionRepository(AppDbContext context) : base(context) { }
        public override async Task<IReadOnlyList<StockTransaction>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(st => st.Product)
                .Include(st => st.Warehouse)
                .Include(st => st.CreatedByUser)
                .OrderByDescending(st => st.TransactionDate)
                .ToListAsync(ct);
        }
        public override void Update(StockTransaction entity)
        {
            throw new InvalidOperationException(
                "StockTransaction is an append-only ledger. Add a reversing transaction instead of updating.");
        }
        public async Task<IReadOnlyList<StockTransaction>> GetByProductAsync(int productId, int? warehouseId = null, CancellationToken ct = default)
        {
            IQueryable<StockTransaction> query = _dbSet
                .AsNoTracking()
                .Include(st => st.Product)
                .Include(st => st.Warehouse)
                .Include(st => st.CreatedByUser)
                .Where(st => st.ProductId == productId);

            if (warehouseId.HasValue)
                query = query.Where(st => st.WarehouseId == warehouseId.Value);

            return await query
                .OrderByDescending(st => st.TransactionDate)
                .ToListAsync(ct);
        }
        public async Task<IReadOnlyList<StockTransaction>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(st => st.Product)
                .Include(st => st.Warehouse)
                .Include(st => st.CreatedByUser)
                .Where(st => st.WarehouseId == warehouseId)
                .OrderByDescending(st => st.TransactionDate)
                .ToListAsync(ct);
        }
        public async Task<IReadOnlyList<StockTransaction>> GetByReferenceAsync(string transactionType, int referenceId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(st => st.Product)
                .Include(st => st.Warehouse)
                .Include(st => st.CreatedByUser)
                .Where(st => st.TransactionType == transactionType && st.ReferenceId == referenceId)
                .OrderByDescending(st => st.TransactionDate)
                .ToListAsync(ct);
        }
        public async Task<IReadOnlyList<StockTransaction>> GetByDateRangeAsync(DateTime from, DateTime to, int? warehouseId = null, CancellationToken ct = default)
        {
            IQueryable<StockTransaction> query = _dbSet
                .AsNoTracking()
                .Include(st => st.Product)
                .Include(st => st.Warehouse)
                .Include(st => st.CreatedByUser)
                .Where(st => st.TransactionDate >= from && st.TransactionDate < to);

            if (warehouseId.HasValue)
                query = query.Where(st => st.WarehouseId == warehouseId.Value);

            return await query
                .OrderByDescending(st => st.TransactionDate)
                .ToListAsync(ct);
        }
        public Task<int> GetNetQuantityAsync(int productId, int warehouseId, CancellationToken ct = default)
            => _dbSet
                .Where(st => st.ProductId == productId && st.WarehouseId == warehouseId)
                .SumAsync(st => st.QuantityChanged, ct);
        public async Task AddRangeAsync(IEnumerable<StockTransaction> transactions, CancellationToken ct = default)
        {
            await _dbSet.AddRangeAsync(transactions, ct);
        }
    }
}