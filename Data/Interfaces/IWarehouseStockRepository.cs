using IMS.Entities;

namespace Data.Interfaces
{
    public interface IWarehouseStockRepository : IGenericRepository<WarehouseStock>
    {
        Task<WarehouseStock?> GetByWarehouseAndProductAsync(int warehouseId, int productId, CancellationToken ct = default);
        Task<IReadOnlyList<WarehouseStock>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default);
        Task<IReadOnlyList<WarehouseStock>> GetByProductAsync(int productId, CancellationToken ct = default);
        Task<int> GetTotalOnHandAsync(int productId, CancellationToken ct = default);

        Task<IReadOnlyList<(int ProductId, string ProductName, string SKU, int ReorderLevel, int TotalOnHand)>> GetLowStockAsync(CancellationToken ct = default);
        // Keys are product IDs. Products with no stock rows are absent.
        Task<IReadOnlyDictionary<int, int>> GetTotalsByProductAsync(CancellationToken ct = default);
    }
}