using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class SalesOrderRepository : GenericRepository<SalesOrder>, ISalesOrderRepository
    {
        public SalesOrderRepository(AppDbContext context) : base(context) { }
        public override async Task<IReadOnlyList<SalesOrder>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(so => so.Customer)
                .Include(so => so.SourceWarehouse)
                .Include(so => so.CreatedByUser)
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(soi => soi.Product)
                .OrderByDescending(so => so.OrderDate)
                .ToListAsync(ct);
        }

        public Task<SalesOrder?> GetByIdWithItemsAsync(int id, CancellationToken ct = default)
            => _dbSet
                .Include(so => so.Customer)
                .Include(so => so.SourceWarehouse)
                .Include(so => so.CreatedByUser)
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(soi => soi.Product)
                .FirstOrDefaultAsync(so => so.Id == id, ct);

        public async Task<IReadOnlyList<SalesOrder>> GetAllWithItemsAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(so => so.Customer)
                .Include(so => so.SourceWarehouse)
                .Include(so => so.CreatedByUser)
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(soi => soi.Product)
                .OrderByDescending(so => so.OrderDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<SalesOrder>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(so => so.Customer)
                .Include(so => so.SourceWarehouse)
                .Include(so => so.CreatedByUser)
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(soi => soi.Product)
                .Where(so => so.CustomerId == customerId)
                .OrderByDescending(so => so.OrderDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<SalesOrder>> GetBySourceWarehouseIdAsync(int warehouseId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(so => so.Customer)
                .Include(so => so.SourceWarehouse)
                .Include(so => so.CreatedByUser)
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(soi => soi.Product)
                .Where(so => so.SourceWarehouseId == warehouseId)
                .OrderByDescending(so => so.OrderDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<SalesOrder>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(so => so.Customer)
                .Include(so => so.SourceWarehouse)
                .Include(so => so.CreatedByUser)
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(soi => soi.Product)
                .Where(so => so.Status == status)
                .OrderByDescending(so => so.OrderDate)
                .ToListAsync(ct);
        }

        // 'to' is exclusive. Pass the day after the last day you want included.
        public async Task<IReadOnlyList<SalesOrder>> GetByOrderDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(so => so.Customer)
                .Include(so => so.SourceWarehouse)
                .Include(so => so.CreatedByUser)
                .Include(so => so.SalesOrderItems)
                    .ThenInclude(soi => soi.Product)
                .Where(so => so.OrderDate >= from && so.OrderDate < to)
                .OrderByDescending(so => so.OrderDate)
                .ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => _dbSet.AnyAsync(so => so.Id == id, ct);

        // Items are not in the generic set, so this one uses the context directly.
        public async Task<bool> RemoveItemAsync(int salesOrderId, int itemId, CancellationToken ct = default)
        {
            var item = await _context.SalesOrderItems
                .FirstOrDefaultAsync(i => i.Id == itemId && i.SalesOrderId == salesOrderId, ct);

            if (item is null)
                return false;

            _context.SalesOrderItems.Remove(item);
            return true;
        }
    }
}