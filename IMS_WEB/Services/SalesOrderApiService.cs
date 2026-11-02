using System.Net;
using System.Net.Http.Json;
using DTOs.SalesOrders;
using IMS_WEB.Services.Interface;

namespace IMS_WEB.Services
{
    public class SalesOrderApiService : ISalesOrderApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SalesOrderApiService> _logger;

        public SalesOrderApiService(HttpClient httpClient, ILogger<SalesOrderApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<SalesOrderSummaryDto>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<SalesOrderSummaryDto>>("salesorders", ct);
                return response ?? Enumerable.Empty<SalesOrderSummaryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sales orders from API.");
                throw;
            }
        }

        public async Task<SalesOrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"salesorders/{id}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SalesOrderDto>(cancellationToken: ct);
        }

        public async Task<IEnumerable<SalesOrderSummaryDto>> GetByCustomerAsync(int customerId, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<SalesOrderSummaryDto>>(
                    $"salesorders/customer/{customerId}", ct);
                return response ?? Enumerable.Empty<SalesOrderSummaryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sales orders for customer {CustomerId}.", customerId);
                return Enumerable.Empty<SalesOrderSummaryDto>();
            }
        }

        public async Task<IEnumerable<SalesOrderSummaryDto>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<SalesOrderSummaryDto>>(
                    $"salesorders/status/{Uri.EscapeDataString(status)}", ct);
                return response ?? Enumerable.Empty<SalesOrderSummaryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sales orders with status '{Status}'.", status);
                return Enumerable.Empty<SalesOrderSummaryDto>();
            }
        }

        public async Task<SalesOrderDto?> CreateAsync(CreateSalesOrderRequest request, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("salesorders", request, ct);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Failed to create sales order. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SalesOrderDto>(cancellationToken: ct);
        }

        public async Task<SalesOrderDto?> UpdateAsync(int id, UpdateSalesOrderRequest request, CancellationToken ct = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"salesorders/{id}", request, ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<SalesOrderDto>(cancellationToken: ct);
        }

        public async Task<bool> SubmitAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.PatchAsync($"salesorders/{id}/submit", null, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ShipAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.PatchAsync($"salesorders/{id}/ship", null, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CancelAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.PatchAsync($"salesorders/{id}/cancel", null, ct);
            return response.IsSuccessStatusCode;
        }
    }
}
