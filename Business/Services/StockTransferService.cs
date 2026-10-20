using Business.Interfaces;
using Data.Interfaces;
using DTOs.StockTransfers;
using IMS.Entities;

namespace Business.Services
{
    public class StockTransferService : IStockTransferService, IStockTransferItemService
    {
        private readonly IStockTransferRepository _transferRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseStockRepository _warehouseStockRepository;
        private readonly IStockTransactionRepository _transactionRepository;

        public StockTransferService(
            IStockTransferRepository transferRepository,
            IWarehouseRepository warehouseRepository,
            IProductRepository productRepository,
            IWarehouseStockRepository warehouseStockRepository,
            IStockTransactionRepository transactionRepository)
        {
            _transferRepository = transferRepository;
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IReadOnlyList<StockTransferDto>> GetAllAsync(CancellationToken ct = default)
        {
            var transfers = await _transferRepository.GetAllWithItemsAsync(ct);
            return transfers.Select(MapToDto).ToList();
        }

        public async Task<StockTransferDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var transfer = await _transferRepository.GetByIdWithItemsAsync(id, ct);
            return transfer is null ? null : MapToDto(transfer);
        }

        public async Task<IReadOnlyList<StockTransferDto>> GetBySourceWarehouseAsync(int warehouseId, CancellationToken ct = default)
        {
            var transfers = await _transferRepository.GetBySourceWarehouseIdAsync(warehouseId, ct);
            return transfers.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<StockTransferDto>> GetByDestinationWarehouseAsync(int warehouseId, CancellationToken ct = default)
        {
            var transfers = await _transferRepository.GetByDestinationWarehouseIdAsync(warehouseId, ct);
            return transfers.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<StockTransferDto>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            var transfers = await _transferRepository.GetByStatusAsync(status, ct);
            return transfers.Select(MapToDto).ToList();
        }

        public async Task<StockTransferDto> CreateAsync(CreateStockTransferRequest request, string userId, CancellationToken ct = default)
        {
            // Validate warehouses are different
            if (request.SourceWarehouseId == request.DestinationWarehouseId)
                throw new InvalidOperationException("Source and destination warehouses must be different.");

            // Validate warehouses exist
            var sourceWarehouse = await _warehouseRepository.GetByIdAsync(request.SourceWarehouseId, ct);
            if (sourceWarehouse is null)
                throw new InvalidOperationException($"Source warehouse '{request.SourceWarehouseId}' does not exist.");

            var destWarehouse = await _warehouseRepository.GetByIdAsync(request.DestinationWarehouseId, ct);
            if (destWarehouse is null)
                throw new InvalidOperationException($"Destination warehouse '{request.DestinationWarehouseId}' does not exist.");

            // Validate products and no duplicates
            var productIds = new HashSet<int>();
            foreach (var item in request.Items)
            {
                if (!productIds.Add(item.ProductId))
                    throw new InvalidOperationException($"Product '{item.ProductId}' appears multiple times in the transfer.");

                var product = await _productRepository.GetByIdAsync(item.ProductId, ct);
                if (product is null)
                    throw new InvalidOperationException($"Product '{item.ProductId}' does not exist.");
            }

            var transfer = new StockTransfer
            {
                SourceWarehouseId = request.SourceWarehouseId,
                DestinationWarehouseId = request.DestinationWarehouseId,
                TransferDate = request.TransferDate ?? DateTime.UtcNow,
                Status = "Draft",
                Notes = Normalize(request.Notes),
                CreatedByUserId = userId,
                StockTransferItems = request.Items
                    .Select(item => new StockTransferItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    })
                    .ToList()
            };

            await _transferRepository.AddAsync(transfer, ct);
            await _transferRepository.SaveChangesAsync(ct);

            return MapToDto(transfer);
        }

        public async Task<StockTransferDto?> UpdateAsync(int id, UpdateStockTransferRequest request, CancellationToken ct = default)
        {
            var transfer = await _transferRepository.GetByIdWithItemsAsync(id, ct);
            if (transfer is null)
                return null;

            if (transfer.Status != "Draft")
                throw new InvalidOperationException($"Cannot update transfer in '{transfer.Status}' status.");

            // Validate warehouses are different
            if (request.SourceWarehouseId == request.DestinationWarehouseId)
                throw new InvalidOperationException("Source and destination warehouses must be different.");

            // Validate warehouses exist
            var sourceWarehouse = await _warehouseRepository.GetByIdAsync(request.SourceWarehouseId, ct);
            if (sourceWarehouse is null)
                throw new InvalidOperationException($"Source warehouse '{request.SourceWarehouseId}' does not exist.");

            var destWarehouse = await _warehouseRepository.GetByIdAsync(request.DestinationWarehouseId, ct);
            if (destWarehouse is null)
                throw new InvalidOperationException($"Destination warehouse '{request.DestinationWarehouseId}' does not exist.");

            transfer.SourceWarehouseId = request.SourceWarehouseId;
            transfer.DestinationWarehouseId = request.DestinationWarehouseId;
            transfer.TransferDate = request.TransferDate;
            transfer.Notes = Normalize(request.Notes);

            _transferRepository.Update(transfer);
            await _transferRepository.SaveChangesAsync(ct);

            return MapToDto(transfer);
        }

        public async Task<bool> SubmitAsync(int id, string userId, CancellationToken ct = default)
        {
            var transfer = await _transferRepository.GetByIdWithItemsAsync(id, ct);
            if (transfer is null)
                return false;

            if (transfer.Status != "Draft")
                throw new InvalidOperationException($"Cannot submit transfer in '{transfer.Status}' status.");

            if (transfer.StockTransferItems.Count == 0)
                throw new InvalidOperationException("Cannot submit transfer with no items.");

            // Verify sufficient stock in source warehouse
            foreach (var item in transfer.StockTransferItems)
            {
                var stock = await _warehouseStockRepository.GetByWarehouseAndProductAsync(transfer.SourceWarehouseId, item.ProductId, ct);
                if (stock is null || stock.QuantityOnHand < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product '{item.ProductId}' in source warehouse.");
            }

            transfer.Status = "Pending";
            _transferRepository.Update(transfer);
            await _transferRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> ApproveAsync(int id, string userId, CancellationToken ct = default)
        {
            var transfer = await _transferRepository.GetByIdWithItemsAsync(id, ct);
            if (transfer is null)
                return false;

            if (transfer.Status != "Pending")
                throw new InvalidOperationException($"Cannot approve transfer in '{transfer.Status}' status.");

            transfer.Status = "Approved";
            transfer.ApprovedByUserId = userId;
            _transferRepository.Update(transfer);
            await _transferRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> CompleteAsync(int id, CancellationToken ct = default)
        {
            var transfer = await _transferRepository.GetByIdWithItemsAsync(id, ct);
            if (transfer is null)
                return false;

            if (transfer.Status != "Approved")
                throw new InvalidOperationException($"Cannot complete transfer in '{transfer.Status}' status.");

            // Move stock from source to destination
            foreach (var item in transfer.StockTransferItems)
            {
                var sourceStock = await _warehouseStockRepository.GetByWarehouseAndProductAsync(transfer.SourceWarehouseId, item.ProductId, ct);
                if (sourceStock is not null)
                {
                    sourceStock.QuantityOnHand -= item.Quantity;
                    _warehouseStockRepository.Update(sourceStock);
                }

                var destStock = await _warehouseStockRepository.GetByWarehouseAndProductAsync(transfer.DestinationWarehouseId, item.ProductId, ct);
                if (destStock is null)
                {
                    destStock = new WarehouseStock
                    {
                        WarehouseId = transfer.DestinationWarehouseId,
                        ProductId = item.ProductId,
                        QuantityOnHand = 0
                    };
                    await _warehouseStockRepository.AddAsync(destStock, ct);
                }
                else
                {
                    destStock.QuantityOnHand += item.Quantity;
                    _warehouseStockRepository.Update(destStock);
                }

                // Record transactions
                var outTx = new StockTransaction
                {
                    WarehouseId = transfer.SourceWarehouseId,
                    ProductId = item.ProductId,
                    QuantityChanged = -item.Quantity,
                    TransactionType = "TransferOut",
                    TransactionDate = DateTime.UtcNow,
                    ReferenceId = id,
                    CreatedByUserId = transfer.CreatedByUserId
                };

                var inTx = new StockTransaction
                {
                    WarehouseId = transfer.DestinationWarehouseId,
                    ProductId = item.ProductId,
                    QuantityChanged = item.Quantity,
                    TransactionType = "TransferIn",
                    TransactionDate = DateTime.UtcNow,
                    ReferenceId = id,
                    CreatedByUserId = transfer.CreatedByUserId
                };

                await _transactionRepository.AddAsync(outTx, ct);
                await _transactionRepository.AddAsync(inTx, ct);
            }

            transfer.Status = "Completed";
            transfer.CompletedAt = DateTime.UtcNow;
            _transferRepository.Update(transfer);
            await _transferRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> CancelAsync(int id, CancellationToken ct = default)
        {
            var transfer = await _transferRepository.GetByIdWithItemsAsync(id, ct);
            if (transfer is null)
                return false;

            if (transfer.Status != "Draft" && transfer.Status != "Pending")
                throw new InvalidOperationException($"Cannot cancel transfer in '{transfer.Status}' status.");

            transfer.Status = "Cancelled";
            _transferRepository.Update(transfer);
            await _transferRepository.SaveChangesAsync(ct);

            return true;
        }

        private static StockTransferDto MapToDto(StockTransfer transfer) => new()
        {
            Id = transfer.Id,
            TransferDate = transfer.TransferDate,
            Status = transfer.Status,
            Notes = transfer.Notes,
            SourceWarehouseId = transfer.SourceWarehouseId,
            SourceWarehouseName = transfer.SourceWarehouse?.Name ?? string.Empty,
            DestinationWarehouseId = transfer.DestinationWarehouseId,
            DestinationWarehouseName = transfer.DestinationWarehouse?.Name ?? string.Empty,
            CreatedByUserName = transfer.CreatedByUser?.UserName,
            ApprovedByUserName = transfer.ApprovedByUser?.UserName,
            CompletedAt = transfer.CompletedAt,
            Items = transfer.StockTransferItems.Select(item => new StockTransferItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                SKU = item.Product?.SKU ?? string.Empty,
                Quantity = item.Quantity
            }).ToList()
        };

        private static string? Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        // IStockTransferItemService implementation
        public async Task<IReadOnlyList<StockTransferItemDto>> GetByTransferAsync(int transferId, CancellationToken ct = default)
        {
            var transfer = await _transferRepository.GetByIdWithItemsAsync(transferId, ct);
            if (transfer is null)
                return new List<StockTransferItemDto>();

            return transfer.StockTransferItems.Select(item => new StockTransferItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                SKU = item.Product?.SKU ?? string.Empty,
                Quantity = item.Quantity
            }).ToList();
        }
    }
}
