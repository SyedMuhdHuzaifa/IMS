using DTOs.SalesOrders;

namespace Business.Interfaces
{
    public interface ISalesOrderItemsService
    {
        // These operations are typically part of order management, so they are shown here
        // for completeness, but the main service is through the SalesOrderService.

        Task<IReadOnlyList<SalesOrderItemDto>> GetByOrderAsync(int orderId, CancellationToken ct = default);
    }
}
