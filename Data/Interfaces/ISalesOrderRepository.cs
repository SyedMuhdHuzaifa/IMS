using IMS.Entities;

namespace Data.Interfaces
{
    public interface ISalesOrderRepository : IGenericRepository<SalesOrder>
    {
        Task<SalesOrder?> GetByIdWithItemsAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<SalesOrder>> GetAllWithItemsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<SalesOrder>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default);
        Task<IReadOnlyList<SalesOrder>> GetBySourceWarehouseIdAsync(int warehouseId, CancellationToken ct = default);
        Task<IReadOnlyList<SalesOrder>> GetByStatusAsync(string status, CancellationToken ct = default);
        Task<IReadOnlyList<SalesOrder>> GetByOrderDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}