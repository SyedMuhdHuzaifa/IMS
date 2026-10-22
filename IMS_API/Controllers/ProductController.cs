using Business.Interfaces;
using DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace IMS_API.Controllers
{
    [Route("api/products")]
    public class ProductController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false, CancellationToken ct = default)
        {
            var products = await _productService.GetAllAsync(includeInactive, ct);
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var product = await _productService.GetByIdAsync(id, ct);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetByCategory(int categoryId, [FromQuery] bool includeInactive = false, CancellationToken ct = default)
        {
            var products = await _productService.GetByCategoryAsync(categoryId, includeInactive, ct);
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term, CancellationToken ct = default)
        {
            var products = await _productService.SearchAsync(term, ct);
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct = default)
        {
            var createdProduct = await _productService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request, CancellationToken ct = default)
        {
            var updatedProduct = await _productService.UpdateAsync(id, request, ct);
            if (updatedProduct == null)
            {
                return NotFound();
            }

            return Ok(updatedProduct);
        }

        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id, CancellationToken ct = default)
        {
            var success = await _productService.DeactivateAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/reactivate")]
        public async Task<IActionResult> Reactivate(int id, CancellationToken ct = default)
        {
            var success = await _productService.ReactivateAsync(id, ct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
