using DTOs.Suppliers;

namespace IMS_WEB.Services.Interface
{
    public interface ISupplierApiService
    {
        Task<IEnumerable<SupplierDto>> GetAllAsync(CancellationToken ct = default);
        Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> CreateAsync(CreateSupplierRequest dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateSupplierRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
