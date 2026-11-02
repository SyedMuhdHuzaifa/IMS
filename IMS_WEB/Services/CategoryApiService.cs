using DTOs.Categories;
using DTOs.Products;
using IMS_WEB.Services.Interface;
using System.Net;
using System.Net.Http.Json;

namespace IMS_WEB.Services
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryApiService> _logger;

        public CategoryApiService(HttpClient httpClient, ILogger<CategoryApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDto>>("categories", ct);
                return result ?? Enumerable.Empty<CategoryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories from API.");
                return Enumerable.Empty<CategoryDto>();
            }
        }

        public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"Categories/{id}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken: ct);

        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
        {
            var response = await _httpClient.PostAsJsonAsync("Categories", request, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken: ct) ?? throw new InvalidOperationException("Failed to deserialize created category.");
        }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"Categories/{id}", request, ct);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken: ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var response = await _httpClient.DeleteAsync($"Categories/{id}", ct);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            response.EnsureSuccessStatusCode();
            return true;
        }

    }
}
