using DTOs.SalesOrders;
using IMS_WEB.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS_WEB.Controllers
{
    [Authorize]
    public class SalesOrderController : Controller
    {
        private readonly ISalesOrderApiService _salesOrderApiService;
        private readonly ICustomerApiService _customerApiService;
        private readonly IWarehouseApiService _warehouseApiService;
        private readonly IProductApiServices _productApiService;
        private readonly ILogger<SalesOrderController> _logger;

        public SalesOrderController(
            ISalesOrderApiService salesOrderApiService,
            ICustomerApiService customerApiService,
            IWarehouseApiService warehouseApiService,
            IProductApiServices productApiService,
            ILogger<SalesOrderController> logger)
        {
            _salesOrderApiService = salesOrderApiService;
            _customerApiService = customerApiService;
            _warehouseApiService = warehouseApiService;
            _productApiService = productApiService;
            _logger = logger;
        }

        // GET: /SalesOrder?status=...&customerId=...
        public async Task<IActionResult> Index(string? status, int? customerId, CancellationToken ct)
        {
            try
            {
                IEnumerable<SalesOrderSummaryDto> orders;

                if (!string.IsNullOrWhiteSpace(status))
                {
                    orders = await _salesOrderApiService.GetByStatusAsync(status, ct);
                }
                else if (customerId.HasValue && customerId.Value > 0)
                {
                    orders = await _salesOrderApiService.GetByCustomerAsync(customerId.Value, ct);
                }
                else
                {
                    orders = await _salesOrderApiService.GetAllAsync(ct);
                }

                // If both filters are present, narrow in-memory
                if (!string.IsNullOrWhiteSpace(status) && customerId.HasValue && customerId.Value > 0)
                {
                    // If we fetched by status, filter by customer as well
                    var customerOrders = await _salesOrderApiService.GetByCustomerAsync(customerId.Value, ct);
                    var customerOrderIds = customerOrders.Select(o => o.Id).ToHashSet();
                    orders = orders.Where(o => customerOrderIds.Contains(o.Id));
                }

                var customers = await _customerApiService.GetAllAsync(ct);
                ViewBag.Customers = new SelectList(customers, "Id", "Name", customerId);
                ViewBag.SelectedStatus = status;
                ViewBag.SelectedCustomerId = customerId;

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load sales orders.");
                TempData["ErrorMessage"] = "Could not load sales orders from the backend service.";
                ViewBag.Customers = new SelectList(Enumerable.Empty<object>());
                return View(Enumerable.Empty<SalesOrderSummaryDto>());
            }
        }

        // GET: /SalesOrder/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var order = await _salesOrderApiService.GetByIdAsync(id, ct);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: /SalesOrder/Create
        [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropdownsAsync(ct);
            return View(new CreateSalesOrderRequest { OrderDate = DateTime.Today });
        }

        // POST: /SalesOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
        public async Task<IActionResult> Create(CreateSalesOrderRequest request, CancellationToken ct)
        {
            // Filter out any empty line items (e.g., added and left unselected)
            var validItems = request.Items?
                .Where(i => i.ProductId > 0 && i.Quantity > 0)
                .ToList() ?? new List<SalesOrderItemRequest>();

            request.Items = validItems;

            if (!request.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one product item with a quantity greater than zero is required.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(ct);
                return View(request);
            }

            var created = await _salesOrderApiService.CreateAsync(request, ct);
            if (created == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to create sales order. Please verify stock availability and try again.");
                await PopulateDropdownsAsync(ct);
                return View(request);
            }

            TempData["SuccessMessage"] = $"Sales Order #SO-{created.Id:D5} created successfully in Draft status.";
            return RedirectToAction(nameof(Details), new { id = created.Id });
        }

        // POST: /SalesOrder/Submit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
        public async Task<IActionResult> Submit(int id, CancellationToken ct)
        {
            var success = await _salesOrderApiService.SubmitAsync(id, ct);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Order submitted successfully for fulfillment." : "Failed to submit order. Please verify order status.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /SalesOrder/Ship/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
        public async Task<IActionResult> Ship(int id, CancellationToken ct)
        {
            var success = await _salesOrderApiService.ShipAsync(id, ct);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Order marked as Shipped! Warehouse inventory has been deducted." : "Failed to ship order. Check available warehouse stock levels.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /SalesOrder/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        {
            var success = await _salesOrderApiService.CancelAsync(id, ct);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Sales Order has been cancelled." : "Failed to cancel order.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // ─── Helpers ────────────────────────────────────────────────────────
        private async Task PopulateDropdownsAsync(CancellationToken ct)
        {
            var customers = await _customerApiService.GetAllAsync(ct);
            var warehouses = await _warehouseApiService.GetAllAsync(includeInactive: false, ct);
            var products = await _productApiService.GetAllAsync(includeInactive: false, ct);

            ViewBag.Customers = new SelectList(customers, "Id", "Name");
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
            ViewBag.Products = products.ToList();
        }
    }
}
