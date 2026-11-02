using DTOs.Products;
using IMS_WEB.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS_WEB.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductApiServices _productApiService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductApiServices productApiService,
            ICategoryApiService categoryApiService,
            ILogger<ProductController> logger)
        {
            _productApiService = productApiService;
            _categoryApiService = categoryApiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? search,
            int? categoryId,
            bool includeInactive = false,
            CancellationToken ct = default)
        {
            try
            {
                IEnumerable<ProductDto> products;

                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    products = await _productApiService.GetByCategoryAsync(categoryId.Value, includeInactive, ct);
                }
                else
                {
                    products = await _productApiService.GetAllAsync(includeInactive, ct);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var term = search.Trim();
                    products = products.Where(p =>
                        p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        p.SKU.Contains(term, StringComparison.OrdinalIgnoreCase));
                }

                var categories = await _categoryApiService.GetAllAsync(ct);
                ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);

                ViewBag.Search = search;
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.IncludeInactive = includeInactive;

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load products.");
                TempData["ErrorMessage"] = "Could not load products from the backend service.";
                ViewBag.Categories = new SelectList(Enumerable.Empty<object>());
                return View(Enumerable.Empty<ProductDto>());
            }
        }


        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var product = await _productApiService.GetByIdAsync(id, ct);
            if (product == null) return NotFound();
            return View(product);
        }


        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await LoadCategoriesAsync(ct);
            return View(new CreateProductRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(CreateProductRequest dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(ct);
                return View(dto);
            }

            var success = await _productApiService.CreateAsync(dto, ct);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to create product. Please try again.");
                await LoadCategoriesAsync(ct);
                return View(dto);
            }

            TempData["SuccessMessage"] = $"Product '{dto.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var product = await _productApiService.GetByIdAsync(id, ct);
            if (product == null) return NotFound();

            var request = new UpdateProductRequest
            {
                Name = product.Name,
                Description = product.Description,
                PurchasePrice = product.PurchasePrice,
                SellingPrice = product.SellingPrice,
                ReorderLevel = product.ReorderLevel,
                CategoryId = product.CategoryId
            };

            ViewData["ProductId"] = id;
            ViewData["ProductSKU"] = product.SKU;
            ViewData["ProductName"] = product.Name;
            await LoadCategoriesAsync(ct, product.CategoryId);
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, UpdateProductRequest dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = id;
                await LoadCategoriesAsync(ct, dto.CategoryId);
                return View(dto);
            }

            var success = await _productApiService.UpdateAsync(id, dto, ct);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to update product. Please try again.");
                ViewData["ProductId"] = id;
                await LoadCategoriesAsync(ct, dto.CategoryId);
                return View(dto);
            }

            TempData["SuccessMessage"] = $"Product '{dto.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
        {
            var success = await _productApiService.DeactivateAsync(id, ct);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Product deactivated successfully." : "Failed to deactivate product.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Reactivate(int id, CancellationToken ct)
        {
            var success = await _productApiService.ReactivateAsync(id, ct);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Product reactivated successfully." : "Failed to reactivate product.";
            return RedirectToAction(nameof(Index), new { includeInactive = true });
        }

        private async Task LoadCategoriesAsync(CancellationToken ct, int? selectedId = null)
        {
            var categories = await _categoryApiService.GetAllAsync(ct);
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedId);
        }
    }
}
