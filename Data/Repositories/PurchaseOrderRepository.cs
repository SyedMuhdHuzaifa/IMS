using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class PurchaseOrderRepository : GenericRepository<PurchaseOrder>, IPurchaseOrderRepository
    {
        private readonly AppDbContext _context;

        public PurchaseOrderRepository(AppDbContext context): base(context)
        {
            _context = context;
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int id, bool includeItems = false, CancellationToken ct = default)
        {
            IQueryable<PurchaseOrder> query = _context.PurchaseOrders;

            if (includeItems)
                query = query.Include(po => po.PurchaseOrderItems);

            return await query.FirstOrDefaultAsync(po => po.Id == id, ct);
        }
        public async Task<IReadOnlyList<PurchaseOrder>> GetAllAsync(bool includeItems = false, CancellationToken ct = default)
        {
            IQueryable<PurchaseOrder> query = _context.PurchaseOrders.AsNoTracking();

            if (includeItems)
                query = query.Include(po => po.PurchaseOrderItems);

            return await query
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<PurchaseOrder>> GetBySupplierIdAsync(int supplierId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrders
                .AsNoTracking()
                .Where(po => po.SupplierId == supplierId)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<PurchaseOrder>> GetByDestinationWarehouseIdAsync(int warehouseId, CancellationToken ct = default)
        {
            return await _context.PurchaseOrders
                .AsNoTracking()
                .Where(po => po.DestinationWarehouseId == warehouseId)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<PurchaseOrder>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            return await _context.PurchaseOrders
                .AsNoTracking()
                .Where(po => po.Status == status)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync(ct);
        }
        public async Task<IReadOnlyList<PurchaseOrder>> GetByOrderDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        {
            return await _context.PurchaseOrders
                .AsNoTracking()
                .Where(po => po.OrderDate >= from && po.OrderDate < to)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => _context.PurchaseOrders.AnyAsync(po => po.Id == id, ct);

        public async Task<bool> RemoveItemAsync(int purchaseOrderId, int itemId, CancellationToken ct = default)
        {
            var item = await _context.PurchaseOrderItems
                .FirstOrDefaultAsync(i => i.Id == itemId && i.PurchaseOrderId == purchaseOrderId, ct);

            if (item is null)
                return false;

            _context.PurchaseOrderItems.Remove(item);
            return true;
        }


    }
}