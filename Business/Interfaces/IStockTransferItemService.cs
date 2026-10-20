using DTOs.StockTransfers;

namespace Business.Interfaces
{
    public interface IStockTransferItemService
    {
        // These operations are typically part of transfer management, so they are shown here
        // for completeness, but the main service is through the StockTransferService.

        Task<IReadOnlyList<StockTransferItemDto>> GetByTransferAsync(int transferId, CancellationToken ct = default);
    }
}
