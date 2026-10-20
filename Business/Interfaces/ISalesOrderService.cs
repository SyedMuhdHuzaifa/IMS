using DTOs.SalesOrders;

namespace Business.Interfaces
{
    public interface ISalesOrderService
    {
        Task<IReadOnlyList<SalesOrderDto>> GetAllAsync(CancellationToken ct = default);

        Task<SalesOrderDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<SalesOrderDto>> GetByCustomerAsync(int customerId, CancellationToken ct = default);

        Task<IReadOnlyList<SalesOrderDto>> GetBySourceWarehouseAsync(int warehouseId, CancellationToken ct = default);

        Task<IReadOnlyList<SalesOrderDto>> GetByStatusAsync(string status, CancellationToken ct = default);

        // Throws if customer or warehouse doesn't exist, products don't exist, or insufficient stock.
        Task<SalesOrderDto> CreateAsync(CreateSalesOrderRequest request, string userId, CancellationToken ct = default);

        // Returns null if the order doesn't exist. Throws if not in Draft status or if validation fails.
        Task<SalesOrderDto?> UpdateAsync(int id, UpdateSalesOrderRequest request, CancellationToken ct = default);

        // Returns false if the order doesn't exist. Throws if order is not in Draft status.
        Task<bool> SubmitAsync(int id, CancellationToken ct = default);

        // Returns false if the order doesn't exist. Throws if order is not in Submitted status.
        Task<bool> ShipAsync(int id, CancellationToken ct = default);

        // Returns false if the order doesn't exist. Throws if order is not in Draft or Submitted.
        Task<bool> CancelAsync(int id, CancellationToken ct = default);
    }
}
