
using Data.Interfaces;
using IMS.Data;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{

    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(
            int id,
            CancellationToken ct = default)
        {
            return await _dbSet.FindAsync(
                new object[] { id },
                ct);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(
            CancellationToken ct = default)
        {
            return await _dbSet.ToListAsync(ct);
        }
        // In GenericRepository<T>
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task AddAsync(
            T entity,
            CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual async Task<int> SaveChangesAsync(
            CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}