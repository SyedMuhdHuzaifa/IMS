using Business.Interfaces;
using Data.Interfaces;
using DTOs.SalesOrders;

namespace Business.Services
{
    public class SalesOrderItemService : ISalesOrderItemsService
    {
        private readonly ISalesOrderRepository _orderRepository;

        public SalesOrderItemService(ISalesOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IReadOnlyList<SalesOrderItemDto>> GetByOrderAsync(int orderId, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId, ct);
            if (order is null)
                return new List<SalesOrderItemDto>();

            return order.SalesOrderItems.Select(item => new SalesOrderItemDto
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
