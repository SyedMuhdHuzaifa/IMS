using DTOs.Categories;

namespace Business.Interfaces
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);

        Task<CategoryDto?> GetByIdAsync(int id, CancellationToken ct = default);

        // Throws if a category with the same name already exists.
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);

        // Returns null if the category doesn't exist. Throws if the new name is taken by another category.
        Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default);

        // Returns false if the category doesn't exist. Throws if it still has products.
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}