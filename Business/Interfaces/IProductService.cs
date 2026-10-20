using DTOs.Products;

namespace Business.Interfaces
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);

        Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<ProductDto>> GetByCategoryAsync(int categoryId, bool includeInactive = false, CancellationToken ct = default);

        Task<IReadOnlyList<ProductDto>> SearchAsync(string term, CancellationToken ct = default);

        // Throws if the SKU is taken or the category doesn't exist.
        Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default);

        // Returns null if the product doesn't exist. Throws if the category doesn't exist.
        Task<ProductDto?> UpdateAsync(int id, UpdateProductRequest request, CancellationToken ct = default);

        // Returns false if the product doesn't exist.
        Task<bool> DeactivateAsync(int id, CancellationToken ct = default);

        // Returns false if the product doesn't exist.
        Task<bool> ReactivateAsync(int id, CancellationToken ct = default);
    }
}