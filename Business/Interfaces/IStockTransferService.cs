using DTOs.StockTransfers;

namespace Business.Interfaces
{
    public interface IStockTransferService
    {
        Task<IReadOnlyList<StockTransferDto>> GetAllAsync(CancellationToken ct = default);

        Task<StockTransferDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<StockTransferDto>> GetBySourceWarehouseAsync(int warehouseId, CancellationToken ct = default);

        Task<IReadOnlyList<StockTransferDto>> GetByDestinationWarehouseAsync(int warehouseId, CancellationToken ct = default);

        Task<IReadOnlyList<StockTransferDto>> GetByStatusAsync(string status, CancellationToken ct = default);

        // Throws if source and destination are the same, warehouses don't exist, products don't exist,
        // or items are duplicated.
        Task<StockTransferDto> CreateAsync(CreateStockTransferRequest request, string userId, CancellationToken ct = default);

        // Returns null if the transfer doesn't exist. Throws if not in Draft status or if warehouses/products don't exist.
        Task<StockTransferDto?> UpdateAsync(int id, UpdateStockTransferRequest request, CancellationToken ct = default);

        // Returns false if the transfer doesn't exist. Throws if stock is insufficient or transfer is not in Draft.
        Task<bool> SubmitAsync(int id, string userId, CancellationToken ct = default);

        // Returns false if the transfer doesn't exist. Throws if transfer is not in Pending status.
        Task<bool> ApproveAsync(int id, string userId, CancellationToken ct = default);

        // Returns false if the transfer doesn't exist. Throws if transfer is not in Approved status.
        Task<bool> CompleteAsync(int id, CancellationToken ct = default);

        // Returns false if the transfer doesn't exist. Throws if transfer is not in Draft or Pending.
        Task<bool> CancelAsync(int id, CancellationToken ct = default);
    }
}
