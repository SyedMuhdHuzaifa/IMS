using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using IMS_WEB.Services.Interface;
using DTOs.Warehouses;

namespace IMS_WEB.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseApiService _warehouseApiService;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(IWarehouseApiService warehouseApiService, ILogger<WarehouseController> logger)
        {
            _warehouseApiService = warehouseApiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            try
            {
                var warehouses = await _warehouseApiService.GetAllAsync(true, ct);
                return View(warehouses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load warehouses.");
                TempData["ErrorMessage"] = "Could not load warehouses from the backend service.";
                return View(Enumerable.Empty<WarehouseDto>());
            }
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var warehouse = await _warehouseApiService.GetByIdAsync(id, ct);
            if (warehouse == null)
                return NotFound();
            return View(warehouse);
        }

        [Authorize(Roles = "Admin,Manager, WarehouseManager")]
        public IActionResult Create()
        {
            return View(new CreateWarehouseRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager, WarehouseManager")]
        public async Task<IActionResult> Create(CreateWarehouseRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(request);
            try
            {
                var createdWarehouse = await _warehouseApiService.CreateAsync(request, ct);
                TempData["SuccessMessage"] =
                    $"Warehouse '{createdWarehouse.Name}' was created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create warehouse.");
                TempData["ErrorMessage"] = "Could not create warehouse.";
                return View(request);
            }
        }

        [Authorize(Roles = "Admin,Manager, WarehouseManager")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var warehouse = await _warehouseApiService.GetByIdAsync(id, ct);
            if (warehouse == null)
                return NotFound();

            var editRequest = new UpdateWarehouseRequest
            {
                Name = warehouse.Name,
                Location = warehouse.Location,
                Description = warehouse.Description,
            };
            ViewData["WarehouseId"] = id;
            return View(editRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager, WarehouseManager")]
        public async Task<IActionResult> Edit(int id, UpdateWarehouseRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewData["WarehouseId"] = id;
                return View(request);
            }
            try
            {
                var updatedWarehouse = await _warehouseApiService.UpdateAsync(id, request, ct);
                if (updatedWarehouse == null)
                {
                    TempData["ErrorMessage"] = "Warehouse not found or could not be updated.";
                    ViewData["WarehouseId"] = id;
                    return View(request);
                }

                TempData["SuccessMessage"] =
                    $"Warehouse '{updatedWarehouse.Name}' was updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update warehouse.");
                TempData["ErrorMessage"] = "Could not update warehouse.";
                ViewData["WarehouseId"] = id;
                return View(request);
            }
        }

        // GET: /Warehouse/Delete/5
        [Authorize(Roles = "Admin,Manager, WarehouseManager")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var warehouse = await _warehouseApiService.GetByIdAsync(id, ct);
            if (warehouse == null)
                return NotFound();

            return View(warehouse);
        }

        // POST: /Warehouse/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager, WarehouseManager")]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            try
            {
                var success = await _warehouseApiService.DeleteAsync(id, ct);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Could not delete the warehouse. Please try again.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["SuccessMessage"] = "Warehouse deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete warehouse.");
                TempData["ErrorMessage"] = "Could not delete the warehouse. It may contain stocks or transactions.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
