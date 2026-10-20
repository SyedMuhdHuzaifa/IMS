using Business.Interfaces;
using Data.Interfaces;
using DTOs.Categories;
using IMS.Entities;

namespace Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        {
            var categories = await _categoryRepository.GetAllWithProductCountsAsync(ct);

            var result = new List<CategoryDto>(categories.Count);
            foreach (var category in categories)
                result.Add(await MapToDtoAsync(category, ct));

            return result;
        }

        public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, ct);
            return category is null ? null : await MapToDtoAsync(category, ct);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
        {
            var name = request.Name.Trim();

            if (await _categoryRepository.NameExistsAsync(name, null, ct))
                throw new InvalidOperationException($"A category named '{name}' already exists.");

            var category = new Category
            {
                Name = name,
                Description = request.Description?.Trim()
            };

            await _categoryRepository.AddAsync(category, ct);
            await _categoryRepository.SaveChangesAsync(ct);

            return await MapToDtoAsync(category, ct);
        }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, ct);
            if (category is null)
                return null;

            var name = request.Name.Trim();

            if (await _categoryRepository.NameExistsAsync(name, id, ct))
                throw new InvalidOperationException($"A category named '{name}' already exists.");

            category.Name = name;
            category.Description = request.Description?.Trim();

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(ct);

            return await MapToDtoAsync(category, ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, ct);
            if (category is null)
                return false;

            var productCount = await _categoryRepository.GetProductCountAsync(id, ct);
            if (productCount > 0)
                throw new InvalidOperationException(
                    $"Category '{category.Name}' still has {productCount} product(s) and cannot be deleted.");

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task<CategoryDto> MapToDtoAsync(Category category, CancellationToken ct)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = await _categoryRepository.GetProductCountAsync(category.Id, ct)
            };
        }
    }
}