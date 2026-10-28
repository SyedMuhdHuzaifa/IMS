using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class StockTransferRepository : GenericRepository<StockTransfer>, IStockTransferRepository
    {
        public StockTransferRepository(AppDbContext context) : base(context) { }
        public override async Task<IReadOnlyList<StockTransfer>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(st => st.SourceWarehouse)
                .Include(st => st.DestinationWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ApprovedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .OrderByDescending(st => st.TransferDate)
                .ToListAsync(ct);
        }

        public Task<StockTransfer?> GetByIdWithItemsAsync(int id, CancellationToken ct = default)
            => _dbSet
                .Include(st => st.SourceWarehouse)
                .Include(st => st.DestinationWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ApprovedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .FirstOrDefaultAsync(st => st.Id == id, ct);

        public async Task<IReadOnlyList<StockTransfer>> GetAllWithItemsAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(st => st.SourceWarehouse)
                .Include(st => st.DestinationWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ApprovedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .OrderByDescending(st => st.TransferDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<StockTransfer>> GetBySourceWarehouseIdAsync(int warehouseId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(st => st.SourceWarehouse)
                .Include(st => st.DestinationWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ApprovedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .Where(st => st.SourceWarehouseId == warehouseId)
                .OrderByDescending(st => st.TransferDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<StockTransfer>> GetByDestinationWarehouseIdAsync(int warehouseId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(st => st.SourceWarehouse)
                .Include(st => st.DestinationWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ApprovedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .Where(st => st.DestinationWarehouseId == warehouseId)
                .OrderByDescending(st => st.TransferDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<StockTransfer>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(st => st.SourceWarehouse)
                .Include(st => st.DestinationWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ApprovedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .Where(st => st.Status == status)
                .OrderByDescending(st => st.TransferDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<StockTransfer>> GetByTransferDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(st => st.SourceWarehouse)
                .Include(st => st.DestinationWarehouse)
                .Include(st => st.CreatedByUser)
                .Include(st => st.ApprovedByUser)
                .Include(st => st.StockTransferItems)
                    .ThenInclude(sti => sti.Product)
                .Where(st => st.TransferDate >= from && st.TransferDate < to)
                .OrderByDescending(st => st.TransferDate)
                .ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => _dbSet.AnyAsync(st => st.Id == id, ct);
        public async Task<bool> RemoveItemAsync(int stockTransferId, int itemId, CancellationToken ct = default)
        {
            var item = await _context.StockTransferItems
                .FirstOrDefaultAsync(i => i.Id == itemId && i.StockTransferId == stockTransferId, ct);

            if (item is null)
                return false;

            _context.StockTransferItems.Remove(item);
            return true;
        }
    }
}