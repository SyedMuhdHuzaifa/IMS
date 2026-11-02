using System.Net;
using System.Net.Http.Json;
using DTOs.Customers;
using IMS_WEB.Services.Interface;

namespace IMS_WEB.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomerApiService> _logger;

        public CustomerApiService(HttpClient httpClient, ILogger<CustomerApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<CustomerDto>>("customers", ct);
                return response ?? Enumerable.Empty<CustomerDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers from API.");
                throw;
            }
        }

        public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"customers/{id}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CustomerDto>(cancellationToken: ct);
        }

        public async Task<IEnumerable<CustomerDto>> SearchAsync(string term, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<CustomerDto>>(
                    $"customers/search?term={Uri.EscapeDataString(term)}", ct);
                return response ?? Enumerable.Empty<CustomerDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers for term '{Term}'.", term);
                return Enumerable.Empty<CustomerDto>();
            }
        }

        public async Task<bool> CreateAsync(CreateCustomerRequest dto, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("customers", dto, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerRequest dto, CancellationToken ct = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"customers/{id}", dto, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.DeleteAsync($"customers/{id}", ct);
            return response.IsSuccessStatusCode;
        }
    }
}
