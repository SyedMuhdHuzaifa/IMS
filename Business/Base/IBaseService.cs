namespace Business.Base
{
    /// <summary>
    /// Interface for base service with standard CRUD operations and DTO mapping.
    /// Implementations should inherit from BaseService<TEntity, TDto, TCreateRequest, TUpdateRequest>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TDto">The DTO type for responses.</typeparam>
    /// <typeparam name="TCreateRequest">The DTO type for create requests.</typeparam>
    /// <typeparam name="TUpdateRequest">The DTO type for update requests.</typeparam>
    public interface IBaseService<TEntity, TDto, TCreateRequest, TUpdateRequest> where TEntity : class where TDto : class where TCreateRequest : class where TUpdateRequest : class
    {
        /// <summary>
        /// Gets all entities as DTOs.
        /// </summary>
        Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets an entity by ID as a DTO.
        /// </summary>
        Task<TDto?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Creates a new entity from a create request and returns the created DTO.
        /// </summary>
        Task<TDto> CreateAsync(TCreateRequest request, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing entity and returns the updated DTO, or null if not found.
        /// </summary>
        Task<TDto?> UpdateAsync(int id, TUpdateRequest request, CancellationToken ct = default);

        /// <summary>
        /// Deletes an entity by ID. Returns true if successful, false if not found.
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
