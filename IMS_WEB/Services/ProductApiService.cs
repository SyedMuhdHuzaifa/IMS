using System.Net;
using System.Net.Http.Json;
using DTOs.Products;
using IMS_WEB.Services.Interface;

namespace IMS_WEB.Services
{
    public class ProductApiService : IProductApiServices
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductApiService> _logger;

        public ProductApiService(HttpClient httpClient, ILogger<ProductApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"products?includeInactive={includeInactive}", ct);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>(cancellationToken: ct) ?? Enumerable.Empty<ProductDto>();

                _logger.LogError("Failed to get products. Status: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products.");
                throw;
            }
        }

        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"products/{id}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: ct);
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId, bool includeInactive = false, CancellationToken ct = default)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>(
                    $"products/category/{categoryId}?includeInactive={includeInactive}", ct);
                return result ?? Enumerable.Empty<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products by category {CategoryId}.", categoryId);
                return Enumerable.Empty<ProductDto>();
            }
        }

        public async Task<IEnumerable<ProductDto>> SearchAsync(string term, CancellationToken ct = default)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>(
                    $"products/search?term={Uri.EscapeDataString(term)}", ct);
                return result ?? Enumerable.Empty<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products for term '{Term}'.", term);
                return Enumerable.Empty<ProductDto>();
            }
        }

        public async Task<bool> CreateAsync(CreateProductRequest dto, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("products", dto, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductRequest dto, CancellationToken ct = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"products/{id}", dto, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.PatchAsync($"products/{id}/deactivate", null, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReactivateAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.PatchAsync($"products/{id}/reactivate", null, ct);
            return response.IsSuccessStatusCode;
        }
    }
}
