using DTOs.Warehouses;

namespace Business.Interfaces
{
    public interface IWarehouseService
    {
        Task<IReadOnlyList<WarehouseDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);

        Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken ct = default);

        // Throws if the code is taken.
        Task<WarehouseDto> CreateAsync(CreateWarehouseRequest request, CancellationToken ct = default);

        // Returns null if the warehouse doesn't exist.
        Task<WarehouseDto?> UpdateAsync(int id, UpdateWarehouseRequest request, CancellationToken ct = default);

        // Returns false if the warehouse doesn't exist.
        Task<bool> DeactivateAsync(int id, CancellationToken ct = default);

        // Returns false if the warehouse doesn't exist.
        Task<bool> ReactivateAsync(int id, CancellationToken ct = default);
    }
}
