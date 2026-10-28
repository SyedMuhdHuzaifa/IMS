using DTOs.WarehouseStocks;

namespace Business.Interfaces
{
    public interface IWarehouseStockService
    {
        // Get stock by warehouse, product, or both. May be paginated if results are large.
        Task<IReadOnlyList<WarehouseStockDto>> QueryAsync(int? warehouseId = null, int? productId = null, bool excludeZero = false, CancellationToken ct = default);

        // Get stock by warehouse.
        Task<IReadOnlyList<WarehouseStockDto>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default);

        // Get stock by product.
        Task<IReadOnlyList<WarehouseStockDto>> GetByProductAsync(int productId, CancellationToken ct = default);

        // Get total on-hand across all warehouses for a product.
        Task<int> GetTotalOnHandAsync(int productId, CancellationToken ct = default);

        // Get all products below their reorder level.
        Task<IReadOnlyList<LowStockItemDto>> GetLowStockAsync(CancellationToken ct = default);
    }
}
