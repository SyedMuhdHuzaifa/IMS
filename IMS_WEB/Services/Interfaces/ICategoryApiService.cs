using DTOs.Categories;

namespace IMS_WEB.Services.Interface
{
    public interface ICategoryApiService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default);
        Task<CategoryDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);
        Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    }
}
