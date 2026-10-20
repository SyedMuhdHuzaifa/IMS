using Business.Specifications;
using Data.Interfaces;

namespace Business.Services.Generic
{
    /// <summary>
    /// Generic service implementation for simple entities with basic CRUD operations.
    /// Provides out-of-the-box functionality for entities that don't have complex domain logic.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TDto">The DTO type.</typeparam>
    /// <typeparam name="TCreateRequest">The create request DTO.</typeparam>
    /// <typeparam name="TUpdateRequest">The update request DTO.</typeparam>
    public abstract class GenericService<TEntity, TDto, TCreateRequest, TUpdateRequest> : BaseService<TEntity, TDto, TCreateRequest, TUpdateRequest>
        where TEntity : class
        where TDto : class
        where TCreateRequest : class
        where TUpdateRequest : class
    {
        protected GenericService(IGenericRepository<TEntity> repository) : base(repository)
        {
        }

        /// <summary>
        /// Gets all entities matching a specification.
        /// </summary>
        public virtual async Task<IReadOnlyList<TDto>> QueryAsync(Specification<TEntity> specification, CancellationToken ct = default)
        {
            var query = Repository.GetAllAsync(ct);

            // Note: This is a simplified version. For full specification support,
            // you would need to enhance GenericRepository to support IQueryable queries.
            var entities = await query;
            return entities.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Gets entities by a predicate function.
        /// </summary>
        public virtual async Task<IReadOnlyList<TDto>> FindAsync(Func<TEntity, bool> predicate, CancellationToken ct = default)
        {
            var entities = await Repository.GetAllAsync(ct);
            return entities.Where(predicate).Select(MapToDto).ToList();
        }

        /// <summary>
        /// Counts entities by a predicate function.
        /// </summary>
        public virtual async Task<int> CountAsync(Func<TEntity, bool> predicate, CancellationToken ct = default)
        {
            var entities = await Repository.GetAllAsync(ct);
            return entities.Count(predicate);
        }

        /// <summary>
        /// Checks if any entity matches the predicate.
        /// </summary>
        public virtual async Task<bool> AnyAsync(Func<TEntity, bool> predicate, CancellationToken ct = default)
        {
            var entities = await Repository.GetAllAsync(ct);
            return entities.Any(predicate);
        }
    }
}
