using DTOs.Customers;
using IMS_WEB.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS_WEB.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerApiService _customerApiService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerApiService customerApiService, ILogger<CustomerController> logger)
        {
            _customerApiService = customerApiService;
            _logger = logger;
        }

        // GET: /Customer?search=...
        public async Task<IActionResult> Index(string? search, CancellationToken ct)
        {
            try
            {
                IEnumerable<CustomerDto> customers;

                if (!string.IsNullOrWhiteSpace(search))
                {
                    customers = await _customerApiService.SearchAsync(search.Trim(), ct);
                }
                else
                {
                    customers = await _customerApiService.GetAllAsync(ct);
                }

                ViewBag.Search = search;
                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load customers.");
                TempData["ErrorMessage"] = "Could not load customers from the backend service.";
                return View(Enumerable.Empty<CustomerDto>());
            }
        }

        // GET: /Customer/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var customer = await _customerApiService.GetByIdAsync(id, ct);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: /Customer/Create
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View(new CreateCustomerRequest());
        }

        // POST: /Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(CreateCustomerRequest dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var success = await _customerApiService.CreateAsync(dto, ct);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to create customer. Please check the details and try again.");
                return View(dto);
            }

            TempData["SuccessMessage"] = $"Customer '{dto.Name}' was created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Customer/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var customer = await _customerApiService.GetByIdAsync(id, ct);
            if (customer == null)
            {
                return NotFound();
            }

            var request = new UpdateCustomerRequest
            {
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };

            ViewData["CustomerId"] = id;
            ViewData["CustomerName"] = customer.Name;
            return View(request);
        }

        // POST: /Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, UpdateCustomerRequest dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CustomerId"] = id;
                return View(dto);
            }

            var success = await _customerApiService.UpdateAsync(id, dto, ct);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to update customer. Please try again.");
                ViewData["CustomerId"] = id;
                return View(dto);
            }

            TempData["SuccessMessage"] = $"Customer '{dto.Name}' was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Customer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _customerApiService.DeleteAsync(id, ct);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Customer deleted successfully." : "Failed to delete customer. The customer may have existing sales orders.";

            return RedirectToAction(nameof(Index));
        }
    }
}
