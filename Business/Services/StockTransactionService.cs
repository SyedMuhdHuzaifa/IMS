using Business.Interfaces;
using Data.Interfaces;
using DTOs.StockTransactions;
using IMS.Entities;

namespace Business.Services
{
    public class StockTransactionService : IStockTransactionService
    {
        private readonly IStockTransactionRepository _transactionRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseStockRepository _warehouseStockRepository;

        public StockTransactionService(
            IStockTransactionRepository transactionRepository,
            IProductRepository productRepository,
            IWarehouseRepository warehouseRepository,
            IWarehouseStockRepository warehouseStockRepository)
        {
            _transactionRepository = transactionRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseStockRepository = warehouseStockRepository;
        }

        public async Task<IReadOnlyList<StockTransactionDto>> GetByProductAsync(int productId, int? warehouseId = null, CancellationToken ct = default)
        {
            var transactions = await _transactionRepository.GetByProductAsync(productId, warehouseId, ct);
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<StockTransactionDto>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default)
        {
            var transactions = await _transactionRepository.GetByWarehouseAsync(warehouseId, ct);
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<StockTransactionDto>> GetByDateRangeAsync(DateTime from, DateTime to, int? warehouseId = null, CancellationToken ct = default)
        {
            var transactions = await _transactionRepository.GetByDateRangeAsync(from, to, warehouseId, ct);
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<StockTransactionDto> RecordAdjustmentAsync(StockAdjustmentRequest request, string userId, CancellationToken ct = default)
        {
            if (request.QuantityChange == 0)
                throw new InvalidOperationException("Quantity change cannot be zero.");

            // Validate product exists
            var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
            if (product is null)
                throw new InvalidOperationException($"Product '{request.ProductId}' does not exist.");

            // Validate warehouse exists
            var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, ct);
            if (warehouse is null)
                throw new InvalidOperationException($"Warehouse '{request.WarehouseId}' does not exist.");

            // Get or create warehouse stock record
            var stock = await _warehouseStockRepository.GetByWarehouseAndProductAsync(request.WarehouseId, request.ProductId, ct);
            if (stock is null)
            {
                stock = new WarehouseStock
                {
                    WarehouseId = request.WarehouseId,
                    ProductId = request.ProductId,
                    QuantityOnHand = 0
                };
                await _warehouseStockRepository.AddAsync(stock, ct);
            }

            // Update stock quantity
            stock.QuantityOnHand += request.QuantityChange;
            if (stock.QuantityOnHand < 0)
                throw new InvalidOperationException("Stock adjustment would result in negative inventory.");

            _warehouseStockRepository.Update(stock);

            // Record transaction
            var transaction = new StockTransaction
            {
                WarehouseId = request.WarehouseId,
                ProductId = request.ProductId,
                QuantityChanged = request.QuantityChange,
                TransactionType = "Adjustment",
                TransactionDate = DateTime.UtcNow,
                ReferenceId = null,
                CreatedByUserId = userId
            };

            await _transactionRepository.AddAsync(transaction, ct);
            await _transactionRepository.SaveChangesAsync(ct);

            return MapToDto(transaction);
        }

        private static StockTransactionDto MapToDto(StockTransaction transaction) => new()
        {
            Id = transaction.Id,
            TransactionDate = transaction.TransactionDate,
            QuantityChanged = transaction.QuantityChanged,
            TransactionType = transaction.TransactionType,
            ReferenceId = transaction.ReferenceId,
            ProductId = transaction.ProductId,
            ProductName = transaction.Product?.Name ?? string.Empty,
            SKU = transaction.Product?.SKU ?? string.Empty,
            WarehouseId = transaction.WarehouseId,
            WarehouseName = transaction.Warehouse?.Name ?? string.Empty,
            CreatedByUserName = transaction.CreatedByUser?.UserName ?? string.Empty
        };
    }
}
