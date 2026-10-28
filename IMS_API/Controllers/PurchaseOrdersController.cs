using Business.Interfaces;
using DTOs.PurchaseOrders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Authorize]
    public class PurchaseOrdersController : BaseApiController
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var orders = await _purchaseOrderService.GetAllAsync(ct);
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var order = await _purchaseOrderService.GetByIdAsync(id, ct);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("supplier/{supplierId:int}")]
        public async Task<IActionResult> GetBySupplier(int supplierId, CancellationToken ct = default)
        {
            var orders = await _purchaseOrderService.GetBySupplierAsync(supplierId, ct);
            return Ok(orders);
        }

        [HttpGet("warehouse/{warehouseId:int}")]
        public async Task<IActionResult> GetByDestinationWarehouse(int warehouseId, CancellationToken ct = default)
        {
            var orders = await _purchaseOrderService.GetByDestinationWarehouseAsync(warehouseId, ct);
            return Ok(orders);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status, CancellationToken ct = default)
        {
            var orders = await _purchaseOrderService.GetByStatusAsync(status, ct);
            return Ok(orders);
        }

        [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderRequest request, CancellationToken ct = default)
        {
            var userId = GetCurrentUserId();
            var createdOrder = await _purchaseOrderService.CreateAsync(request, userId, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseOrderRequest request, CancellationToken ct = default)
        {
            var updatedOrder = await _purchaseOrderService.UpdateAsync(id, request, ct);
            if (updatedOrder == null)
            {
                return NotFound();
            }

            return Ok(updatedOrder);
        }

        [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
        [HttpPatch("{id:int}/submit")]
        public async Task<IActionResult> Submit(int id, CancellationToken ct = default)
        {
            var success = await _purchaseOrderService.SubmitAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
        [HttpPatch("{id:int}/receive")]
        public async Task<IActionResult> Receive(int id, CancellationToken ct = default)
        {
            var success = await _purchaseOrderService.ReceiveAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct = default)
        {
            var success = await _purchaseOrderService.CancelAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
