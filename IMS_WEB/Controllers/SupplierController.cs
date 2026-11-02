using DTOs.Suppliers;
using IMS_WEB.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS_WEB.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly ISupplierApiService _supplierApiService;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(ISupplierApiService supplierApiService, ILogger<SupplierController> logger)
        {
            _supplierApiService = supplierApiService;
            _logger = logger;
        }

        // GET: /Supplier
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            try
            {
                var suppliers = await _supplierApiService.GetAllAsync(ct);
                return View(suppliers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load suppliers.");
                TempData["ErrorMessage"] = "Could not load suppliers from the backend service.";
                return View(Enumerable.Empty<SupplierDto>());
            }
        }

        // GET: /Supplier/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var supplier = await _supplierApiService.GetByIdAsync(id, ct);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // GET: /Supplier/Create
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View(new CreateSupplierRequest());
        }

        // POST: /Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(CreateSupplierRequest dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var success = await _supplierApiService.CreateAsync(dto, ct);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to create supplier. Please try again.");
                return View(dto);
            }

            TempData["SuccessMessage"] = $"Supplier '{dto.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Supplier/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var supplier = await _supplierApiService.GetByIdAsync(id, ct);
            if (supplier == null)
                return NotFound();

            // Map SupplierDto → UpdateSupplierRequest so the form is pre-filled
            var request = new UpdateSupplierRequest
            {
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address
            };

            ViewData["SupplierId"] = id;
            ViewData["SupplierName"] = supplier.Name;
            return View(request);
        }

        // POST: /Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, UpdateSupplierRequest dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewData["SupplierId"] = id;
                return View(dto);
            }

            var success = await _supplierApiService.UpdateAsync(id, dto, ct);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to update supplier. Please try again.");
                ViewData["SupplierId"] = id;
                return View(dto);
            }

            TempData["SuccessMessage"] = $"Supplier '{dto.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Supplier/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _supplierApiService.DeleteAsync(id, ct);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Supplier deleted successfully." : "Failed to delete supplier.";

            return RedirectToAction(nameof(Index));
        }
    }
}
