using DTOs.PurchaseOrders;
using DTOs.Suppliers;
using IMS_WEB.Services.Interface;
using System.Net;
using System.Net.Http.Json;

namespace IMS_WEB.Services
{
    public class PurchaseOrderApiService : IPurchaseOrderApiService
    {
        private readonly HttpClient _httpclinet;
        private readonly ILogger<PurchaseOrderApiService> _logger;

        public PurchaseOrderApiService(HttpClient httpClinet, ILogger<PurchaseOrderApiService> logger)
        {
            _httpclinet = httpClinet;
            _logger = logger;
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var response = await _httpclinet.GetFromJsonAsync<IEnumerable<PurchaseOrderDto>>("purchaseorders", ct);
                return response ?? Enumerable.Empty<PurchaseOrderDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching purchase orders from API.");
                return Enumerable.Empty<PurchaseOrderDto>();
            }
        }

        public async Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpclinet.GetAsync($"purchaseorders/{id}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PurchaseOrderDto>(cancellationToken: ct);
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetBySupplierAsync(int supplierId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpclinet.GetAsync($"purchaseorders/supplier/{supplierId}", ct);
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return Enumerable.Empty<PurchaseOrderDto>();

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<PurchaseOrderDto>>(cancellationToken: ct)
                       ?? Enumerable.Empty<PurchaseOrderDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching purchase orders for supplier {SupplierId}.", supplierId);
                return Enumerable.Empty<PurchaseOrderDto>();
            }
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpclinet.GetAsync($"purchaseorders/warehouse/{warehouseId}", ct);
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return Enumerable.Empty<PurchaseOrderDto>();

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<PurchaseOrderDto>>(cancellationToken: ct)
                       ?? Enumerable.Empty<PurchaseOrderDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching purchase orders for warehouse {WarehouseId}.", warehouseId);
                return Enumerable.Empty<PurchaseOrderDto>();
            }
        }

        public async Task<IEnumerable<PurchaseOrderDto>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpclinet.GetFromJsonAsync<IEnumerable<PurchaseOrderDto>>(
                    $"purchaseorders/status/{Uri.EscapeDataString(status)}", ct);
                return response ?? Enumerable.Empty<PurchaseOrderDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching purchase orders with status '{Status}'.", status);
                return Enumerable.Empty<PurchaseOrderDto>();
            }
        }

        public async Task<PurchaseOrderDto?> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpclinet.PostAsJsonAsync("purchaseorders", request, ct);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(ct);
                    _logger.LogWarning("Failed to create purchase order. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<PurchaseOrderDto>(cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating purchase order.");
                return null;
            }
        }

        public async Task<PurchaseOrderDto?> UpdateAsync(int id, UpdatePurchaseOrderRequest request, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpclinet.PutAsJsonAsync($"purchaseorders/{id}", request, ct);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(ct);
                    _logger.LogWarning("Failed to update purchase order {Id}. Status: {StatusCode}, Error: {Error}", id, response.StatusCode, error);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<PurchaseOrderDto>(cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating purchase order {Id}.", id);
                return null;
            }
        }

        public async Task<bool> SubmitAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpclinet.PatchAsync($"purchaseorders/{id}/submit", null, ct);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting purchase order {Id}.", id);
                return false;
            }
        }

        public async Task<bool> ReceiveAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpclinet.PatchAsync($"purchaseorders/{id}/receive", null, ct);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving purchase order {Id}.", id);
                return false;
            }
        }

        public async Task<bool> CancelAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpclinet.PatchAsync($"purchaseorders/{id}/cancel", null, ct);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling purchase order {Id}.", id);
                return false;
            }
        }
    }
}
