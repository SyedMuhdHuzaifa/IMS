using DTOs.StockTransactions;

namespace Business.Interfaces
{
    public interface IStockTransactionService
    {
        Task<IReadOnlyList<StockTransactionDto>> GetByProductAsync(int productId, int? warehouseId = null, CancellationToken ct = default);

        Task<IReadOnlyList<StockTransactionDto>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default);

        Task<IReadOnlyList<StockTransactionDto>> GetByDateRangeAsync(DateTime from, DateTime to, int? warehouseId = null, CancellationToken ct = default);
        Task<StockTransactionDto> RecordAdjustmentAsync(StockAdjustmentRequest request, string userId, CancellationToken ct = default);
    }
}
