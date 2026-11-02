using DTOs.PurchaseOrders;
using DTOs.Suppliers;
using DTOs.Warehouses;
using IMS_WEB.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS_WEB.Controllers
{
    [Authorize]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderApiService _purchaseOrderApiService;
        private readonly ISupplierApiService _supplierApiService;
        private readonly IWarehouseApiService _warehouseApiService;
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(
            IPurchaseOrderApiService purchaseOrderApiService,
            ISupplierApiService supplierApiService,
            IWarehouseApiService warehouseApiService,
            ILogger<PurchaseOrderController> logger)
        {
            _purchaseOrderApiService = purchaseOrderApiService;
            _supplierApiService = supplierApiService;
            _warehouseApiService = warehouseApiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? search, int? supplierId, string? status, CancellationToken ct = default)
        {
            try
            {
                IEnumerable<PurchaseOrderDto> purchaseOrders;

                if (!string.IsNullOrEmpty(status))
                {
                    purchaseOrders = await _purchaseOrderApiService.GetByStatusAsync(status, ct);
                }
                else if (supplierId.HasValue && supplierId.Value > 0)
                {
                    purchaseOrders = await _purchaseOrderApiService.GetBySupplierAsync(supplierId.Value, ct);
                }
                else
                {
                    purchaseOrders = await _purchaseOrderApiService.GetAllAsync(ct);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var term = search.Trim().ToLower();
                    purchaseOrders = purchaseOrders.Where(po =>
                        po.SupplierName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        po.DestinationWarehouseName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        po.CreatedByUserName.Contains(term, StringComparison.OrdinalIgnoreCase));
                }

                var suppliers = await _supplierApiService.GetAllAsync(ct);
                ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name", supplierId);

                var statuses = new List<string> { "Draft", "Submitted", "Received", "Cancelled" };
                ViewBag.Statuses = new SelectList(statuses, status);

                ViewBag.Search = search;
                ViewBag.SelectedSupplierId = supplierId;
                ViewBag.SelectedStatus = status;

                return View(purchaseOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load purchase orders.");
                TempData["ErrorMessage"] = "Could not load purchase orders from the backend service.";
                ViewBag.Suppliers = new SelectList(Enumerable.Empty<object>());
                ViewBag.Statuses = new SelectList(Enumerable.Empty<object>());
                return View(Enumerable.Empty<PurchaseOrderDto>());
            }
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            try
            {
                var purchaseOrder = await _purchaseOrderApiService.GetByIdAsync(id, ct);
                if (purchaseOrder == null)
                    return NotFound();

                return View(purchaseOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load purchase order details.");
                TempData["ErrorMessage"] = "Could not load purchase order details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Admin,Manager,Buyer")]
        public async Task<IActionResult> Create(CancellationToken ct = default)
        {
            try
            {
                await LoadSupplierAndWarehouseAsync(ct);
                return View(new CreatePurchaseOrderRequest());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load create form.");
                TempData["ErrorMessage"] = "Could not load the create form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Buyer")]
        public async Task<IActionResult> Create(CreatePurchaseOrderRequest dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                await LoadSupplierAndWarehouseAsync(ct);
                return View(dto);
            }

            try
            {
                var result = await _purchaseOrderApiService.CreateAsync(dto, ct);
                if (result == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create purchase order. Please try again.");
                    await LoadSupplierAndWarehouseAsync(ct);
                    return View(dto);
                }

                TempData["SuccessMessage"] = $"Purchase order created successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create purchase order.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the purchase order.");
                await LoadSupplierAndWarehouseAsync(ct);
                return View(dto);
            }
        }

        [Authorize(Roles = "Admin,Manager,Buyer")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            try
            {
                var purchaseOrder = await _purchaseOrderApiService.GetByIdAsync(id, ct);
                if (purchaseOrder == null)
                    return NotFound();

                // Only Draft orders can be edited
                if (purchaseOrder.Status != "Draft")
                {
                    TempData["WarningMessage"] = "Only draft purchase orders can be edited.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var request = new UpdatePurchaseOrderRequest
                {
                    SupplierId = purchaseOrder.SupplierId,
                    DestinationWarehouseId = purchaseOrder.DestinationWarehouseId,
                    OrderDate = purchaseOrder.OrderDate,
                    Items = purchaseOrder.Items.Select(i => new PurchaseOrderItemRequest
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToArray()
                };

                ViewData["PurchaseOrderId"] = id;
                ViewData["PurchaseOrderStatus"] = purchaseOrder.Status;
                await LoadSupplierAndWarehouseAsync(ct, purchaseOrder.SupplierId, purchaseOrder.DestinationWarehouseId);
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load edit form.");
                TempData["ErrorMessage"] = "Could not load the purchase order for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Buyer")]
        public async Task<IActionResult> Edit(int id, UpdatePurchaseOrderRequest dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                ViewData["PurchaseOrderId"] = id;
                await LoadSupplierAndWarehouseAsync(ct, dto.SupplierId, dto.DestinationWarehouseId);
                return View(dto);
            }

            try
            {
                var result = await _purchaseOrderApiService.UpdateAsync(id, dto, ct);
                if (result == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update purchase order. Please try again.");
                    ViewData["PurchaseOrderId"] = id;
                    await LoadSupplierAndWarehouseAsync(ct, dto.SupplierId, dto.DestinationWarehouseId);
                    return View(dto);
                }

                TempData["SuccessMessage"] = "Purchase order updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update purchase order.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the purchase order.");
                ViewData["PurchaseOrderId"] = id;
                await LoadSupplierAndWarehouseAsync(ct, dto.SupplierId, dto.DestinationWarehouseId);
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Buyer")]
        public async Task<IActionResult> Submit(int id, CancellationToken ct = default)
        {
            try
            {
                var success = await _purchaseOrderApiService.SubmitAsync(id, ct);
                TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                    success ? "Purchase order submitted successfully." : "Failed to submit purchase order.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to submit purchase order.");
                TempData["ErrorMessage"] = "An error occurred while submitting the purchase order.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Buyer")]
        public async Task<IActionResult> Receive(int id, CancellationToken ct = default)
        {
            try
            {
                var success = await _purchaseOrderApiService.ReceiveAsync(id, ct);
                TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                    success ? "Purchase order received successfully." : "Failed to receive purchase order.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to receive purchase order.");
                TempData["ErrorMessage"] = "An error occurred while receiving the purchase order.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Buyer")]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct = default)
        {
            try
            {
                var success = await _purchaseOrderApiService.CancelAsync(id, ct);
                TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                    success ? "Purchase order cancelled successfully." : "Failed to cancel purchase order.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel purchase order.");
                TempData["ErrorMessage"] = "An error occurred while cancelling the purchase order.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task LoadSupplierAndWarehouseAsync(
            CancellationToken ct,
            int? selectedSupplierId = null,
            int? selectedWarehouseId = null)
        {
            try
            {
                var suppliers = await _supplierApiService.GetAllAsync(ct);
                var warehouses = await _warehouseApiService.GetAllAsync(includeInactive: false, ct);

                ViewBag.SupplierId = new SelectList(suppliers, "Id", "Name", selectedSupplierId);
                ViewBag.DestinationWarehouseId = new SelectList(warehouses, "Id", "Name", selectedWarehouseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load suppliers and warehouses.");
            }
        }
    }
}
