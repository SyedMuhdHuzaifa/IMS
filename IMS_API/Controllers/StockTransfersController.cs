using Business.Interfaces;
using DTOs.StockTransfers;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    public class StockTransfersController : BaseApiController
    {
        private readonly IStockTransferService _stockTransferService;

        public StockTransfersController(IStockTransferService stockTransferService)
        {
            _stockTransferService = stockTransferService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var transfers = await _stockTransferService.GetAllAsync(ct);
            return Ok(transfers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var transfer = await _stockTransferService.GetByIdAsync(id, ct);
            if (transfer == null)
            {
                return NotFound();
            }

            return Ok(transfer);
        }

        [HttpGet("source/{warehouseId:int}")]
        public async Task<IActionResult> GetBySourceWarehouse(int warehouseId, CancellationToken ct = default)
        {
            var transfers = await _stockTransferService.GetBySourceWarehouseAsync(warehouseId, ct);
            return Ok(transfers);
        }

        [HttpGet("destination/{warehouseId:int}")]
        public async Task<IActionResult> GetByDestinationWarehouse(int warehouseId, CancellationToken ct = default)
        {
            var transfers = await _stockTransferService.GetByDestinationWarehouseAsync(warehouseId, ct);
            return Ok(transfers);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status, CancellationToken ct = default)
        {
            var transfers = await _stockTransferService.GetByStatusAsync(status, ct);
            return Ok(transfers);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockTransferRequest request, CancellationToken ct = default)
        {
            var userId = GetCurrentUserId();
            var createdTransfer = await _stockTransferService.CreateAsync(request, userId, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdTransfer.Id }, createdTransfer);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStockTransferRequest request, CancellationToken ct = default)
        {
            var updatedTransfer = await _stockTransferService.UpdateAsync(id, request, ct);
            if (updatedTransfer == null)
            {
                return NotFound();
            }

            return Ok(updatedTransfer);
        }

        [HttpPatch("{id:int}/submit")]
        public async Task<IActionResult> Submit(int id, CancellationToken ct = default)
        {
            var userId = GetCurrentUserId();
            var success = await _stockTransferService.SubmitAsync(id, userId, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id, CancellationToken ct = default)
        {
            var userId = GetCurrentUserId();
            var success = await _stockTransferService.ApproveAsync(id, userId, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/complete")]
        public async Task<IActionResult> Complete(int id, CancellationToken ct = default)
        {
            var success = await _stockTransferService.CompleteAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct = default)
        {
            var success = await _stockTransferService.CancelAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
