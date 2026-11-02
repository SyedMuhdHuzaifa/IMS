using DTOs.Products;

namespace IMS_WEB.Services.Interface
{
    public interface IProductApiServices
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId, bool includeInactive = false, CancellationToken ct = default);
        Task<IEnumerable<ProductDto>> SearchAsync(string term, CancellationToken ct = default);
        Task<bool> CreateAsync(CreateProductRequest dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateProductRequest dto, CancellationToken ct = default);
        Task<bool> DeactivateAsync(int id, CancellationToken ct = default);
        Task<bool> ReactivateAsync(int id, CancellationToken ct = default);
    }
}
