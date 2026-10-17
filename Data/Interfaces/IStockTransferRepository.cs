using IMS.Entities;

namespace Data.Interfaces
{
    public interface IStockTransferRepository : IGenericRepository<StockTransfer>
    {
        Task<StockTransfer?> GetByIdWithItemsAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<StockTransfer>> GetAllWithItemsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<StockTransfer>> GetBySourceWarehouseIdAsync(int warehouseId, CancellationToken ct = default);
        Task<IReadOnlyList<StockTransfer>> GetByDestinationWarehouseIdAsync(int warehouseId, CancellationToken ct = default);
        Task<IReadOnlyList<StockTransfer>> GetByStatusAsync(string status, CancellationToken ct = default);
        Task<IReadOnlyList<StockTransfer>> GetByTransferDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

        Task<bool> RemoveItemAsync(int stockTransferId, int itemId, CancellationToken ct = default);
    }
}