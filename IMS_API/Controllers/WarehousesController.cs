using Business.Interfaces;
using DTOs.Warehouses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Authorize]
    public class WarehousesController : BaseApiController
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false, CancellationToken ct = default)
        {
            var warehouses = await _warehouseService.GetAllAsync(includeInactive, ct);
            return Ok(warehouses);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var warehouse = await _warehouseService.GetByIdAsync(id, ct);
            if (warehouse == null)
            {
                return NotFound();
            }

            return Ok(warehouse);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseRequest request, CancellationToken ct = default)
        {
            var createdWarehouse = await _warehouseService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdWarehouse.Id }, createdWarehouse);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWarehouseRequest request, CancellationToken ct = default)
        {
            var updatedWarehouse = await _warehouseService.UpdateAsync(id, request, ct);
            if (updatedWarehouse == null)
            {
                return NotFound();
            }

            return Ok(updatedWarehouse);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id, CancellationToken ct = default)
        {
            var success = await _warehouseService.DeactivateAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id:int}/reactivate")]
        public async Task<IActionResult> Reactivate(int id, CancellationToken ct = default)
        {
            var success = await _warehouseService.ReactivateAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
