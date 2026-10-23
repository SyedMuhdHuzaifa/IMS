using Business.Interfaces;
using Data.Interfaces;
using DTOs.WarehouseStocks;

namespace Business.Services
{
    public class WarehouseStockService : IWarehouseStockService
    {
        private readonly IWarehouseStockRepository _warehouseStockRepository;

        public WarehouseStockService(IWarehouseStockRepository warehouseStockRepository)
        {
            _warehouseStockRepository = warehouseStockRepository;
        }

        public async Task<IReadOnlyList<WarehouseStockDto>> QueryAsync(int? warehouseId = null, int? productId = null, bool excludeZero = false, CancellationToken ct = default)
        {
            IReadOnlyList<WarehouseStockDto> stocks;

            if (warehouseId.HasValue && productId.HasValue)
            {
                // Get specific stock record
                var stock = await _warehouseStockRepository.GetByWarehouseAndProductAsync(warehouseId.Value, productId.Value, ct);
                stocks = stock is not null ? new List<WarehouseStockDto> { MapToDto(stock) } : new List<WarehouseStockDto>();
            }
            else if (warehouseId.HasValue)
            {
                // Get all stocks for a warehouse
                var warehouseStocks = await _warehouseStockRepository.GetByWarehouseAsync(warehouseId.Value, ct);
                stocks = warehouseStocks.Select(MapToDto).ToList();
            }
            else if (productId.HasValue)
            {
                // Get all stocks for a product
                var productStocks = await _warehouseStockRepository.GetByProductAsync(productId.Value, ct);
                stocks = productStocks.Select(MapToDto).ToList();
            }
            else
            {
                // Get all stocks
                var allStocks = await _warehouseStockRepository.GetAllAsync(ct);
                stocks = allStocks.Select(MapToDto).ToList();
            }

            // Filter out zero stock if requested
            if (excludeZero)
                stocks = stocks.Where(s => s.QuantityOnHand > 0).ToList();

            return stocks;
        }

        public async Task<IReadOnlyList<WarehouseStockDto>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default)
        {
            var stocks = await _warehouseStockRepository.GetByWarehouseAsync(warehouseId, ct);
            return stocks.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<WarehouseStockDto>> GetByProductAsync(int productId, CancellationToken ct = default)
        {
            var stocks = await _warehouseStockRepository.GetByProductAsync(productId, ct);
            return stocks.Select(MapToDto).ToList();
        }

        public async Task<int> GetTotalOnHandAsync(int productId, CancellationToken ct = default)
        {
            return await _warehouseStockRepository.GetTotalOnHandAsync(productId, ct);
        }

        public async Task<IReadOnlyList<LowStockItemDto>> GetLowStockAsync(CancellationToken ct = default)
        {
            var lowStocks = await _warehouseStockRepository.GetLowStockAsync(ct);

            return lowStocks
                .Select(item => new LowStockItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    SKU = item.SKU,
                    ReorderLevel = item.ReorderLevel,
                    TotalOnHand = item.TotalOnHand,
                    ShortBy = Math.Max(0, item.ReorderLevel - item.TotalOnHand)
                })
                .ToList();
        }

        private static WarehouseStockDto MapToDto(IMS.Entities.WarehouseStock stock) => new()
        {
            Id = stock.Id,
            WarehouseId = stock.WarehouseId,
            WarehouseCode = stock.Warehouse?.Code ?? string.Empty,
            WarehouseName = stock.Warehouse?.Name ?? string.Empty,
            ProductId = stock.ProductId,
            ProductName = stock.Product?.Name ?? string.Empty,
            SKU = stock.Product?.SKU ?? string.Empty,
            QuantityOnHand = stock.QuantityOnHand
        };
    }
}
