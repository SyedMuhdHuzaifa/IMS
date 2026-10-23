using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context) { }

        public override async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(ct);
        }

        public async Task<Customer?> GetByEmailAsync(string email, int? excludeId = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var normalized = email.Trim().ToLower();

            return await _dbSet.FirstOrDefaultAsync(c =>
                c.Email != null &&
                c.Email.ToLower() == normalized &&
                (!excludeId.HasValue || c.Id != excludeId.Value), ct);
        }

        public async Task<IReadOnlyList<Customer>> SearchByNameAsync(string term, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Array.Empty<Customer>();

            term = term.Trim();

            return await _dbSet
                .AsNoTracking()
                .Where(c => c.Name.Contains(term))
                .OrderBy(c => c.Name)
                .ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
            => _dbSet.AnyAsync(c => c.Id == id, ct);

        public Task<int> GetSalesOrderCountAsync(int customerId, CancellationToken ct = default)
            => _context.SalesOrders.CountAsync(so => so.CustomerId == customerId, ct);

        public async Task<IReadOnlyDictionary<int, int>> GetSalesOrderCountsAsync(CancellationToken ct = default)
        {
            return await _context.SalesOrders
                .AsNoTracking()
                .GroupBy(so => so.CustomerId)
                .Select(g => new { CustomerId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CustomerId, x => x.Count, ct);
        }
    }
}