using System.Net;
using System.Net.Http.Json;
using DTOs.Warehouses;
using IMS_WEB.Controllers;
using IMS_WEB.Services.Interface;

namespace IMS_WEB.Services
{
    public class WarehouseApiService : IWarehouseApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WarehouseApiService> _logger;

        public WarehouseApiService(HttpClient httpClient, ILogger<WarehouseApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<WarehouseDto>>(
                    $"warehouses?includeInactive={includeInactive}", ct);
                return result ?? Enumerable.Empty<WarehouseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching warehouses from API.");
                return Enumerable.Empty<WarehouseDto>();
            }
        }

        public async Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"warehouses/{id}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<WarehouseDto>(cancellationToken: ct);
        }

        public async Task<WarehouseDto> CreateAsync(CreateWarehouseRequest request, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("warehouses", request, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<WarehouseDto>(cancellationToken: ct) 
                   ?? throw new InvalidOperationException("Failed to deserialize the created warehouse.");
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.DeleteAsync($"warehouses/{id}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<WarehouseDto?> UpdateAsync(int id, UpdateWarehouseRequest request, CancellationToken ct = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"warehouses/{id}", request, ct);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<WarehouseDto>(cancellationToken: ct);
        }
    }
}
