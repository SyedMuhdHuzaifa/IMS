using Data.Interfaces;

namespace Business.Base
{
    /// <summary>
    /// Base service class for standard CRUD operations with automatic DTO mapping.
    /// Reduces boilerplate code by handling GetAll, GetById, Create, Update, Delete, and SaveChanges.
    /// Derived services only need to implement mappings and entity-specific validation logic.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TDto">The DTO type for responses.</typeparam>
    /// <typeparam name="TCreateRequest">The DTO type for create requests.</typeparam>
    /// <typeparam name="TUpdateRequest">The DTO type for update requests.</typeparam>
    public abstract class BaseService<TEntity, TDto, TCreateRequest, TUpdateRequest> : IBaseService<TEntity, TDto, TCreateRequest, TUpdateRequest>
        where TEntity : class
        where TDto : class
        where TCreateRequest : class
        where TUpdateRequest : class
    {
        protected readonly IGenericRepository<TEntity> Repository;

        protected BaseService(IGenericRepository<TEntity> repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Gets all entities as DTOs.
        /// </summary>
        public virtual async Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await Repository.GetAllAsync(ct);
            return entities.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Gets an entity by ID as a DTO.
        /// </summary>
        public virtual async Task<TDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await Repository.GetByIdAsync(id, ct);
            return entity is null ? null : MapToDto(entity);
        }

        /// <summary>
        /// Creates a new entity from a create request.
        /// </summary>
        public virtual async Task<TDto> CreateAsync(TCreateRequest request, CancellationToken ct = default)
        {
            ValidateCreateRequest(request);
            var entity = MapToEntity(request);
            await Repository.AddAsync(entity, ct);
            await Repository.SaveChangesAsync(ct);
            return MapToDto(entity);
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        public virtual async Task<TDto?> UpdateAsync(int id, TUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await Repository.GetByIdAsync(id, ct);
            if (entity is null)
                return null;

            ValidateUpdateRequest(request);
            MapToEntity(request, entity);
            Repository.Update(entity);
            await Repository.SaveChangesAsync(ct);
            return MapToDto(entity);
        }

        /// <summary>
        /// Deletes an entity by ID.
        /// </summary>
        public virtual async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await Repository.GetByIdAsync(id, ct);
            if (entity is null)
                return false;

            ValidateDelete(entity);
            Repository.Delete(entity);
            await Repository.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Maps an entity to a DTO. Must be implemented by derived classes.
        /// </summary>
        protected abstract TDto MapToDto(TEntity entity);

        /// <summary>
        /// Maps a create request to an entity. Must be implemented by derived classes.
        /// </summary>
        protected abstract TEntity MapToEntity(TCreateRequest request);

        /// <summary>
        /// Maps an update request to an entity (modifying existing entity). Must be implemented by derived classes.
        /// </summary>
        protected abstract void MapToEntity(TUpdateRequest request, TEntity entity);

        /// <summary>
        /// Validates a create request. Override to add business logic validation.
        /// </summary>
        protected virtual void ValidateCreateRequest(TCreateRequest request)
        {
            // Override in derived class to add validation
        }

        /// <summary>
        /// Validates an update request. Override to add business logic validation.
        /// </summary>
        protected virtual void ValidateUpdateRequest(TUpdateRequest request)
        {
            // Override in derived class to add validation
        }

        /// <summary>
        /// Validates delete preconditions. Override to prevent deletion of protected entities.
        /// </summary>
        protected virtual void ValidateDelete(TEntity entity)
        {
            // Override in derived class to add validation (e.g., cascade checks)
        }
    }
}
