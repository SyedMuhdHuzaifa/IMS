using Business.Interfaces;
using Data.Interfaces;
using DTOs.PurchaseOrders;
using IMS.Entities;

namespace Business.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _orderRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseStockRepository _warehouseStockRepository;
        private readonly IStockTransactionRepository _transactionRepository;

        public PurchaseOrderService(
            IPurchaseOrderRepository orderRepository,
            ISupplierRepository supplierRepository,
            IWarehouseRepository warehouseRepository,
            IProductRepository productRepository,
            IWarehouseStockRepository warehouseStockRepository,
            IStockTransactionRepository transactionRepository)
        {
            _orderRepository = orderRepository;
            _supplierRepository = supplierRepository;
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IReadOnlyList<PurchaseOrderDto>> GetAllAsync(CancellationToken ct = default)
        {
            var orders = await _orderRepository.GetAllAsync(true, ct);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, true, ct);
            return order is null ? null : MapToDto(order);
        }

        public async Task<IReadOnlyList<PurchaseOrderDto>> GetBySupplierAsync(int supplierId, CancellationToken ct = default)
        {
            var orders = await _orderRepository.GetBySupplierIdAsync(supplierId, ct);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<PurchaseOrderDto>> GetByDestinationWarehouseAsync(int warehouseId, CancellationToken ct = default)
        {
            var orders = await _orderRepository.GetByDestinationWarehouseIdAsync(warehouseId, ct);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<PurchaseOrderDto>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            var orders = await _orderRepository.GetByStatusAsync(status, ct);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderRequest request, string userId, CancellationToken ct = default)
        {
            // Validate supplier exists
            var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, ct);
            if (supplier is null)
                throw new InvalidOperationException($"Supplier '{request.SupplierId}' does not exist.");

            // Validate warehouse exists and is active
            var warehouse = await _warehouseRepository.GetByIdAsync(request.DestinationWarehouseId, ct);
            if (warehouse is null)
                throw new InvalidOperationException($"Destination warehouse '{request.DestinationWarehouseId}' does not exist.");
            if (!warehouse.IsActive)
                throw new InvalidOperationException($"Destination warehouse '{warehouse.Name}' is inactive.");

            // Validate products
            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, ct);
                if (product is null)
                    throw new InvalidOperationException($"Product '{item.ProductId}' does not exist.");
                if (!product.IsActive)
                    throw new InvalidOperationException($"Product '{product.Name}' is inactive.");
            }

            var totalAmount = request.Items.Sum(item => item.Quantity * item.UnitPrice);

            var order = new PurchaseOrder
            {
                SupplierId = request.SupplierId,
                OrderDate = request.OrderDate ?? DateTime.UtcNow,
                Status = "Draft",
                TotalAmount = totalAmount,
                DestinationWarehouseId = request.DestinationWarehouseId,
                CreatedByUserId = userId,
                PurchaseOrderItems = request.Items
                    .Select(item => new PurchaseOrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    })
                    .ToList()
            };

            await _orderRepository.AddAsync(order, ct);
            await _orderRepository.SaveChangesAsync(ct);

            return MapToDto(order);
        }

        public async Task<PurchaseOrderDto?> UpdateAsync(int id, UpdatePurchaseOrderRequest request, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, true, ct);
            if (order is null)
                return null;

            if (order.Status != "Draft")
                throw new InvalidOperationException($"Cannot update order in '{order.Status}' status.");

            // Validate supplier exists
            var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, ct);
            if (supplier is null)
                throw new InvalidOperationException($"Supplier '{request.SupplierId}' does not exist.");

            // Validate warehouse exists and is active
            var warehouse = await _warehouseRepository.GetByIdAsync(request.DestinationWarehouseId, ct);
            if (warehouse is null)
                throw new InvalidOperationException($"Destination warehouse '{request.DestinationWarehouseId}' does not exist.");
            if (!warehouse.IsActive)
                throw new InvalidOperationException($"Destination warehouse '{warehouse.Name}' is inactive.");

            // Validate products
            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, ct);
                if (product is null)
                    throw new InvalidOperationException($"Product '{item.ProductId}' does not exist.");
                if (!product.IsActive)
                    throw new InvalidOperationException($"Product '{product.Name}' is inactive.");
            }

            order.SupplierId = request.SupplierId;
            order.DestinationWarehouseId = request.DestinationWarehouseId;
            order.OrderDate = request.OrderDate;

            // Replace items
            order.PurchaseOrderItems.Clear();
            foreach (var item in request.Items)
            {
                order.PurchaseOrderItems.Add(new PurchaseOrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            order.TotalAmount = order.PurchaseOrderItems.Sum(item => item.Quantity * item.UnitPrice);

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(ct);

            return MapToDto(order);
        }

        public async Task<bool> SubmitAsync(int id, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, true, ct);
            if (order is null)
                return false;

            if (order.Status != "Draft")
                throw new InvalidOperationException($"Cannot submit order in '{order.Status}' status.");

            order.Status = "Submitted";
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> ReceiveAsync(int id, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, true, ct);
            if (order is null)
                return false;

            if (order.Status != "Submitted")
                throw new InvalidOperationException($"Cannot receive order in '{order.Status}' status.");

            // Add stock to warehouse
            foreach (var item in order.PurchaseOrderItems)
            {
                var stock = await _warehouseStockRepository.GetByWarehouseAndProductAsync(order.DestinationWarehouseId, item.ProductId, ct);
                if (stock is null)
                {
                    stock = new WarehouseStock
                    {
                        WarehouseId = order.DestinationWarehouseId,
                        ProductId = item.ProductId,
                        QuantityOnHand = 0
                    };
                    await _warehouseStockRepository.AddAsync(stock, ct);
                }

                stock.QuantityOnHand += item.Quantity;
                _warehouseStockRepository.Update(stock);

                // Record transaction
                var transaction = new StockTransaction
                {
                    WarehouseId = order.DestinationWarehouseId,
                    ProductId = item.ProductId,
                    QuantityChanged = item.Quantity,
                    TransactionType = "PurchaseReceipt",
                    TransactionDate = DateTime.UtcNow,
                    ReferenceId = id,
                    CreatedByUserId = order.CreatedByUserId
                };

                await _transactionRepository.AddAsync(transaction, ct);
            }

            order.Status = "Received";
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> CancelAsync(int id, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, false, ct);
            if (order is null)
                return false;

            if (order.Status != "Draft" && order.Status != "Submitted")
                throw new InvalidOperationException($"Cannot cancel order in '{order.Status}' status.");

            order.Status = "Cancelled";
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(ct);

            return true;
        }

        private static PurchaseOrderDto MapToDto(PurchaseOrder order) => new()
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            SupplierId = order.SupplierId,
            SupplierName = order.Supplier?.Name ?? string.Empty,
            DestinationWarehouseId = order.DestinationWarehouseId,
            DestinationWarehouseName = order.DestinationWarehouse?.Name ?? string.Empty,
            CreatedByUserName = order.CreatedByUser?.UserName ?? string.Empty,
            Items = order.PurchaseOrderItems.Select(item => new PurchaseOrderItemDto
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
