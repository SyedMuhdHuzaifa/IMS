using Business.Interfaces;
using DTOs.WarehouseStocks;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    public class WarehouseStocksController : BaseApiController
    {
        private readonly IWarehouseStockService _warehouseStockService;

        public WarehouseStocksController(IWarehouseStockService warehouseStockService)
        {
            _warehouseStockService = warehouseStockService;
        }

        [HttpGet]
        public async Task<IActionResult> Query(
            [FromQuery] int? warehouseId,
            [FromQuery] int? productId,
            [FromQuery] bool excludeZero = false,
            CancellationToken ct = default)
        {
            var stocks = await _warehouseStockService.QueryAsync(warehouseId, productId, excludeZero, ct);
            return Ok(stocks);
        }

        [HttpGet("warehouse/{warehouseId:int}")]
        public async Task<IActionResult> GetByWarehouse(int warehouseId, CancellationToken ct = default)
        {
            var stocks = await _warehouseStockService.GetByWarehouseAsync(warehouseId, ct);
            return Ok(stocks);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId, CancellationToken ct = default)
        {
            var stocks = await _warehouseStockService.GetByProductAsync(productId, ct);
            return Ok(stocks);
        }

        [HttpGet("product/{productId:int}/total-on-hand")]
        public async Task<IActionResult> GetTotalOnHand(int productId, CancellationToken ct = default)
        {
            var total = await _warehouseStockService.GetTotalOnHandAsync(productId, ct);
            return Ok(new { ProductId = productId, TotalOnHand = total });
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock(CancellationToken ct = default)
        {
            var lowStockItems = await _warehouseStockService.GetLowStockAsync(ct);
            return Ok(lowStockItems);
        }
    }
}
