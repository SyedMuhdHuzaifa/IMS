using Business.Interfaces;
using DTOs.StockTransactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Authorize]
    public class StockTransactionsController : BaseApiController
    {
        private readonly IStockTransactionService _stockTransactionService;

        public StockTransactionsController(IStockTransactionService stockTransactionService)
        {
            _stockTransactionService = stockTransactionService;
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId, [FromQuery] int? warehouseId, CancellationToken ct = default)
        {
            var transactions = await _stockTransactionService.GetByProductAsync(productId, warehouseId, ct);
            return Ok(transactions);
        }

        [HttpGet("warehouse/{warehouseId:int}")]
        public async Task<IActionResult> GetByWarehouse(int warehouseId, CancellationToken ct = default)
        {
            var transactions = await _stockTransactionService.GetByWarehouseAsync(warehouseId, ct);
            return Ok(transactions);
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int? warehouseId,
            CancellationToken ct = default)
        {
            var transactions = await _stockTransactionService.GetByDateRangeAsync(from, to, warehouseId, ct);
            return Ok(transactions);
        }

        [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
        [HttpPost("adjustment")]
        public async Task<IActionResult> RecordAdjustment([FromBody] StockAdjustmentRequest request, CancellationToken ct = default)
        {
            var userId = GetCurrentUserId();
            var transaction = await _stockTransactionService.RecordAdjustmentAsync(request, userId, ct);
            return Ok(transaction);
        }
    }
}
