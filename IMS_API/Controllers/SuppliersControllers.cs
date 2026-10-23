using Business.Interfaces;
using DTOs.Suppliers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Authorize]
    [Route("api/suppliers")]
    public class SuppliersController : BaseApiController
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var suppliers = await _supplierService.GetAllAsync(ct);
            return Ok(suppliers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var supplier = await _supplierService.GetByIdAsync(id, ct);
            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term, CancellationToken ct = default)
        {
            var suppliers = await _supplierService.SearchAsync(term, ct);
            return Ok(suppliers);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request, CancellationToken ct = default)
        {
            var createdSupplier = await _supplierService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdSupplier.Id }, createdSupplier);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierRequest request, CancellationToken ct = default)
        {
            var updatedSupplier = await _supplierService.UpdateAsync(id, request, ct);
            if (updatedSupplier == null)
            {
                return NotFound();
            }

            return Ok(updatedSupplier);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var deleted = await _supplierService.DeleteAsync(id, ct);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
