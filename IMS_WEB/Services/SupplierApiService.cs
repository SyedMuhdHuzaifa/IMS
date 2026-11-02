using System.Net;
using System.Net.Http.Json;
using DTOs.Suppliers;
using IMS_WEB.Services.Interface;

namespace IMS_WEB.Services
{
    public class SupplierApiService : ISupplierApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SupplierApiService> _logger;

        public SupplierApiService(HttpClient httpClient, ILogger<SupplierApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<SupplierDto>>("suppliers", ct);
                return response ?? Enumerable.Empty<SupplierDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching suppliers from API.");
                throw;
            }
        }

        public async Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"suppliers/{id}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SupplierDto>(cancellationToken: ct);
        }

        public async Task<bool> CreateAsync(CreateSupplierRequest dto, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("suppliers", dto, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, UpdateSupplierRequest dto, CancellationToken ct = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"suppliers/{id}", dto, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.DeleteAsync($"suppliers/{id}", ct);
            return response.IsSuccessStatusCode;
        }
    }
}
