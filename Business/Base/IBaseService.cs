namespace Business.Base
{
    public interface IBaseService<TEntity, TDto, TCreateRequest, TUpdateRequest> where TEntity : class where TDto : class where TCreateRequest : class where TUpdateRequest : class
    {
        Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken ct = default);
        Task<TDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<TDto> CreateAsync(TCreateRequest request, CancellationToken ct = default);
        Task<TDto?> UpdateAsync(int id, TUpdateRequest request, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
