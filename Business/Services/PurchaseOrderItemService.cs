using Business.Interfaces;
using Data.Interfaces;
using DTOs.PurchaseOrders;

namespace Business.Services
{
    public class PurchaseOrderItemService : IPurchaseOrderItemService
    {
        private readonly IPurchaseOrderRepository _orderRepository;

        public PurchaseOrderItemService(IPurchaseOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IReadOnlyList<PurchaseOrderItemDto>> GetByOrderAsync(int orderId, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, true, ct);
            if (order is null)
                return new List<PurchaseOrderItemDto>();

            return order.PurchaseOrderItems.Select(item => new PurchaseOrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                SKU = item.Product?.SKU ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            }).ToList();
        }
    }
}
