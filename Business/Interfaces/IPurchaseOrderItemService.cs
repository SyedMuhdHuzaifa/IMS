using DTOs.PurchaseOrders;

namespace Business.Interfaces
{
    public interface IPurchaseOrderItemService
    {
        // These operations are typically part of order management, so they are shown here
        // for completeness, but the main service is through the PurchaseOrderService.

        Task<IReadOnlyList<PurchaseOrderItemDto>> GetByOrderAsync(int orderId, CancellationToken ct = default);
    }
}
