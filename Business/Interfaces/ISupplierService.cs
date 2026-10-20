using DTOs.Suppliers;

namespace Business.Interfaces
{
    public interface ISupplierService
    {
        Task<IReadOnlyList<SupplierDto>> GetAllAsync(CancellationToken ct = default);

        Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<SupplierDto>> SearchAsync(string term, CancellationToken ct = default);

        // Throws if another supplier already uses the email.
        Task<SupplierDto> CreateAsync(CreateSupplierRequest request, CancellationToken ct = default);

        // Returns null if the supplier doesn't exist. Throws if the email belongs to another supplier.
        Task<SupplierDto?> UpdateAsync(int id, UpdateSupplierRequest request, CancellationToken ct = default);

        // Returns false if the supplier doesn't exist. Throws if the supplier has purchase orders.
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
