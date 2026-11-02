using DTOs.Categories;

namespace Business.Interfaces
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);

        Task<CategoryDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);
        Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}