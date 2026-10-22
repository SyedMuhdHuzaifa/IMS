using Business.Interfaces;
using DTOs.Customers;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    public class CustomersController : BaseApiController
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var customers = await _customerService.GetAllAsync(ct);
            return Ok(customers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var customer = await _customerService.GetByIdAsync(id, ct);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term, CancellationToken ct = default)
        {
            var customers = await _customerService.SearchAsync(term, ct);
            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken ct = default)
        {
            var createdCustomer = await _customerService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, createdCustomer);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest request, CancellationToken ct = default)
        {
            var updatedCustomer = await _customerService.UpdateAsync(id, request, ct);
            if (updatedCustomer == null)
            {
                return NotFound();
            }

            return Ok(updatedCustomer);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var deleted = await _customerService.DeleteAsync(id, ct);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}