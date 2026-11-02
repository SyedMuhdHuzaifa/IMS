using DTOs.Warehouses;

namespace IMS_WEB.Services.Interface
{
    public interface IWarehouseApiService
    {
        Task<IEnumerable<WarehouseDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
        Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<WarehouseDto> CreateAsync(CreateWarehouseRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<WarehouseDto?> UpdateAsync(int id, UpdateWarehouseRequest request, CancellationToken ct = default);
    }
}
