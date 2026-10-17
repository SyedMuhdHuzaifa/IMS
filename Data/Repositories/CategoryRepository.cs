
using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class CategoryRepository
        : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<Category?> GetByNameAsync(
            string name,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            name = name.Trim();

            return await _context.Categories
                .FirstOrDefaultAsync(
                    category => category.Name == name,
                    ct);
        }

        public async Task<bool> ExistsAsync(
            int id,
            CancellationToken ct = default)
        {
            return await _context.Categories
                .AnyAsync(category => category.Id == id, ct);
        }
    }
}
