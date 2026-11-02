using DTOs.Categories;
using IMS_WEB.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS_WEB.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryApiService categoryApiService,
            ILogger<CategoriesController> logger)
        {
            _categoryApiService = categoryApiService;
            _logger = logger;
        }

        // GET: /Categories
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            try
            {
                var categories = await _categoryApiService.GetAllAsync(ct);
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load categories.");
                TempData["ErrorMessage"] = "Could not load categories from the backend service.";
                return View(Enumerable.Empty<CategoryDto>());
            }
        }

        // GET: /Categories/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var category = await _categoryApiService.GetByIdAsync(id, ct);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: /Categories/Create
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View(new CreateCategoryRequest());
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(
            CreateCategoryRequest request,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var category = await _categoryApiService.CreateAsync(request, ct);

                TempData["SuccessMessage"] =
                    $"Category '{category.Name}' was created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category {CategoryName}.", request.Name);
                ModelState.AddModelError(
                    string.Empty,
                    "Could not create the category. Please try again.");

                return View(request);
            }
        }

        // GET: /Categories/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var category = await _categoryApiService.GetByIdAsync(id, ct);

            if (category == null)
            {
                return NotFound();
            }

            var request = new UpdateCategoryRequest
            {
                Name = category.Name,
                Description = category.Description
            };

            ViewData["CategoryId"] = id;
            return View(request);
        }

        // POST: /Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(
            int id,
            UpdateCategoryRequest request,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = id;
                return View(request);
            }

            try
            {
                var category = await _categoryApiService.UpdateAsync(id, request, ct);

                if (category == null)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] =
                    $"Category '{category.Name}' was updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update category {CategoryId}.", id);
                ModelState.AddModelError(
                    string.Empty,
                    "Could not update the category. Please try again.");

                ViewData["CategoryId"] = id;
                return View(request);
            }
        }

        // GET: /Categories/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var category = await _categoryApiService.GetByIdAsync(id, ct);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: /Categories/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                var deleted = await _categoryApiService.DeleteAsync(id, ct);

                if (!deleted)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Category was deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete category {CategoryId}.", id);
                TempData["ErrorMessage"] =
                    "Could not delete the category. It may still be in use by products.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}