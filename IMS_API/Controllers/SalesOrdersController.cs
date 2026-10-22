using Business.Interfaces;
using DTOs.SalesOrders;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    public class SalesOrdersController : BaseApiController
    {
        private readonly ISalesOrderService _salesOrderService;

        public SalesOrdersController(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var orders = await _salesOrderService.GetAllAsync(ct);
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var order = await _salesOrderService.GetByIdAsync(id, ct);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(int customerId, CancellationToken ct = default)
        {
            var orders = await _salesOrderService.GetByCustomerAsync(customerId, ct);
            return Ok(orders);
        }

        [HttpGet("warehouse/{warehouseId:int}")]
        public async Task<IActionResult> GetBySourceWarehouse(int warehouseId, CancellationToken ct = default)
        {
            var orders = await _salesOrderService.GetBySourceWarehouseAsync(warehouseId, ct);
            return Ok(orders);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status, CancellationToken ct = default)
        {
            var orders = await _salesOrderService.GetByStatusAsync(status, ct);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSalesOrderRequest request, CancellationToken ct = default)
        {
            var userId = GetCurrentUserId();
            var createdOrder = await _salesOrderService.CreateAsync(request, userId, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSalesOrderRequest request, CancellationToken ct = default)
        {
            var updatedOrder = await _salesOrderService.UpdateAsync(id, request, ct);
            if (updatedOrder == null)
            {
                return NotFound();
            }

            return Ok(updatedOrder);
        }

        [HttpPatch("{id:int}/submit")]
        public async Task<IActionResult> Submit(int id, CancellationToken ct = default)
        {
            var success = await _salesOrderService.SubmitAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/ship")]
        public async Task<IActionResult> Ship(int id, CancellationToken ct = default)
        {
            var success = await _salesOrderService.ShipAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct = default)
        {
            var success = await _salesOrderService.CancelAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
