using Business.Interfaces;
using Data.Interfaces;
using DTOs.SalesOrders;
using IMS.Entities;

namespace Business.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseStockRepository _warehouseStockRepository;
        private readonly IStockTransactionRepository _transactionRepository;

        public SalesOrderService(
            ISalesOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IWarehouseRepository warehouseRepository,
            IProductRepository productRepository,
            IWarehouseStockRepository warehouseStockRepository,
            IStockTransactionRepository transactionRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IReadOnlyList<SalesOrderDto>> GetAllAsync(CancellationToken ct = default)
        {
            var orders = await _orderRepository.GetAllWithItemsAsync(ct);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<SalesOrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(id, ct);
            return order is null ? null : MapToDto(order);
        }

        public async Task<IReadOnlyList<SalesOrderDto>> GetByCustomerAsync(int customerId, CancellationToken ct = default)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(customerId, ct);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<SalesOrderDto>> GetBySourceWarehouseAsync(int warehouseId, CancellationToken ct = default)
        {
            var orders = await _orderRepository.GetBySourceWarehouseIdAsync(warehouseId, ct);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<SalesOrderDto>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            var orders = await _orderRepository.GetByStatusAsync(status, ct);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<SalesOrderDto> CreateAsync(CreateSalesOrderRequest request, string userId, CancellationToken ct = default)
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, ct);
            if (customer is null)
                throw new InvalidOperationException($"Customer '{request.CustomerId}' does not exist.");

            // Validate warehouse exists
            var warehouse = await _warehouseRepository.GetByIdAsync(request.SourceWarehouseId, ct);
            if (warehouse is null)
                throw new InvalidOperationException($"Source warehouse '{request.SourceWarehouseId}' does not exist.");

            // Validate products and check stock
            decimal totalAmount = 0;
            var items = new List<SalesOrderItem>();

            foreach (var itemRequest in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, ct);
                if (product is null)
                    throw new InvalidOperationException($"Product '{itemRequest.ProductId}' does not exist.");

                // Check stock availability
                var stock = await _warehouseStockRepository.GetByWarehouseAndProductAsync(request.SourceWarehouseId, itemRequest.ProductId, ct);
                if (stock is null || stock.QuantityOnHand < itemRequest.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product '{itemRequest.ProductId}' in warehouse.");

                // Determine unit price
                var unitPrice = itemRequest.UnitPrice ?? product.SellingPrice;

                var item = new SalesOrderItem
                {
                    ProductId = itemRequest.ProductId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = unitPrice
                };

                items.Add(item);
                totalAmount += itemRequest.Quantity * unitPrice;
            }

            var order = new SalesOrder
            {
                CustomerId = request.CustomerId,
                OrderDate = request.OrderDate ?? DateTime.UtcNow,
                Status = "Draft",
                TotalAmount = totalAmount,
                SourceWarehouseId = request.SourceWarehouseId,
                CreatedByUserId = userId,
                SalesOrderItems = items
            };

            await _orderRepository.AddAsync(order, ct);
            await _orderRepository.SaveChangesAsync(ct);

            return MapToDto(order);
        }

        public async Task<SalesOrderDto?> UpdateAsync(int id, UpdateSalesOrderRequest request, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(id, ct);
            if (order is null)
                return null;

            if (order.Status != "Draft")
                throw new InvalidOperationException($"Cannot update order in '{order.Status}' status.");

            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, ct);
            if (customer is null)
                throw new InvalidOperationException($"Customer '{request.CustomerId}' does not exist.");

            // Validate warehouse exists
            var warehouse = await _warehouseRepository.GetByIdAsync(request.SourceWarehouseId, ct);
            if (warehouse is null)
                throw new InvalidOperationException($"Source warehouse '{request.SourceWarehouseId}' does not exist.");

            // Validate products and check stock
            decimal totalAmount = 0;
            var items = new List<SalesOrderItem>();

            foreach (var itemRequest in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, ct);
                if (product is null)
                    throw new InvalidOperationException($"Product '{itemRequest.ProductId}' does not exist.");

                // Check stock availability
                var stock = await _warehouseStockRepository.GetByWarehouseAndProductAsync(request.SourceWarehouseId, itemRequest.ProductId, ct);
                if (stock is null || stock.QuantityOnHand < itemRequest.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product '{itemRequest.ProductId}' in warehouse.");

                // Determine unit price
                var unitPrice = itemRequest.UnitPrice ?? product.SellingPrice;

                var item = new SalesOrderItem
                {
                    ProductId = itemRequest.ProductId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = unitPrice
                };

                items.Add(item);
                totalAmount += itemRequest.Quantity * unitPrice;
            }

            order.CustomerId = request.CustomerId;
            order.SourceWarehouseId = request.SourceWarehouseId;
            order.OrderDate = request.OrderDate;

            // Replace items
            order.SalesOrderItems.Clear();
            foreach (var item in items)
            {
                order.SalesOrderItems.Add(item);
            }

            order.TotalAmount = totalAmount;

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(ct);

            return MapToDto(order);
        }

        public async Task<bool> SubmitAsync(int id, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(id, ct);
            if (order is null)
                return false;

            if (order.Status != "Draft")
                throw new InvalidOperationException($"Cannot submit order in '{order.Status}' status.");

            order.Status = "Submitted";
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> ShipAsync(int id, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(id, ct);
            if (order is null)
                return false;

            if (order.Status != "Submitted")
                throw new InvalidOperationException($"Cannot ship order in '{order.Status}' status.");

            // Deduct stock from warehouse
            foreach (var item in order.SalesOrderItems)
            {
                var stock = await _warehouseStockRepository.GetByWarehouseAndProductAsync(order.SourceWarehouseId, item.ProductId, ct);
                if (stock is not null)
                {
                    stock.QuantityOnHand -= item.Quantity;
                    _warehouseStockRepository.Update(stock);
                }

                // Record transaction
                var transaction = new StockTransaction
                {
                    WarehouseId = order.SourceWarehouseId,
                    ProductId = item.ProductId,
                    QuantityChanged = -item.Quantity,
                    TransactionType = "SalesShipment",
                    TransactionDate = DateTime.UtcNow,
                    ReferenceId = id,
                    CreatedByUserId = order.CreatedByUserId
                };

                await _transactionRepository.AddAsync(transaction, ct);
            }

            order.Status = "Shipped";
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> CancelAsync(int id, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(id, ct);
            if (order is null)
                return false;

            if (order.Status != "Draft" && order.Status != "Submitted")
                throw new InvalidOperationException($"Cannot cancel order in '{order.Status}' status.");

            order.Status = "Cancelled";
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(ct);

            return true;
        }

        private static SalesOrderDto MapToDto(SalesOrder order) => new()
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.Name ?? string.Empty,
            SourceWarehouseId = order.SourceWarehouseId,
            SourceWarehouseName = order.SourceWarehouse?.Name ?? string.Empty,
            CreatedByUserName = order.CreatedByUser?.UserName ?? string.Empty,
            Items = order.SalesOrderItems.Select(item => new SalesOrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                SKU = item.Product?.SKU ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            }).ToList()
        };
    }
}
