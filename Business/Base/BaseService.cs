using Data.Interfaces;

namespace Business.Base
{
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


        public virtual async Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken ct = default)
        {
            var entities = await Repository.GetAllAsync(ct);
            return entities.Select(MapToDto).ToList();
        }

        public virtual async Task<TDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await Repository.GetByIdAsync(id, ct);
            return entity is null ? null : MapToDto(entity);
        }

        public virtual async Task<TDto> CreateAsync(TCreateRequest request, CancellationToken ct = default)
        {
            ValidateCreateRequest(request);
            var entity = MapToEntity(request);
            await Repository.AddAsync(entity, ct);
            await Repository.SaveChangesAsync(ct);
            return MapToDto(entity);
        }


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


        protected abstract TDto MapToDto(TEntity entity);

        protected abstract TEntity MapToEntity(TCreateRequest request);

        protected abstract void MapToEntity(TUpdateRequest request, TEntity entity);
        protected virtual void ValidateCreateRequest(TCreateRequest request)
        {
            // Override in derived class to add validation
        }
        protected virtual void ValidateUpdateRequest(TUpdateRequest request)
        {
            // Override in derived class to add validation
        }
        protected virtual void ValidateDelete(TEntity entity)
        {
            // Override in derived class to add validation (e.g., cascade checks)
        }
    }
}
