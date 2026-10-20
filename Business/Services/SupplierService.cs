using Business.Interfaces;
using Data.Interfaces;
using DTOs.Suppliers;
using IMS.Entities;

namespace Business.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IReadOnlyList<SupplierDto>> GetAllAsync(CancellationToken ct = default)
        {
            var suppliers = await _supplierRepository.GetAllAsync(ct);
            return suppliers.Select(s => MapToDto(s, 0)).ToList();
        }

        public async Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id, ct);
            return supplier is null ? null : MapToDto(supplier, 0);
        }

        public async Task<IReadOnlyList<SupplierDto>> SearchAsync(string term, CancellationToken ct = default)
        {
            var suppliers = await _supplierRepository.SearchByNameAsync(term, ct);
            return suppliers.Select(s => MapToDto(s, 0)).ToList();
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierRequest request, CancellationToken ct = default)
        {
            var email = Normalize(request.Email);

            if (email is not null &&
                await _supplierRepository.GetByEmailAsync(email, ct) is not null)
                throw new InvalidOperationException($"A supplier with email '{email}' already exists.");

            var supplier = new Supplier
            {
                Name = request.Name.Trim(),
                Email = email,
                Phone = Normalize(request.Phone),
                Address = Normalize(request.Address)
            };

            await _supplierRepository.AddAsync(supplier, ct);
            await _supplierRepository.SaveChangesAsync(ct);

            return MapToDto(supplier, 0);
        }

        public async Task<SupplierDto?> UpdateAsync(int id, UpdateSupplierRequest request, CancellationToken ct = default)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id, ct);
            if (supplier is null)
                return null;

            var email = Normalize(request.Email);

            if (email is not null &&
                await _supplierRepository.GetByEmailAsync(email, ct) is not null &&
                supplier.Email != email)
                throw new InvalidOperationException($"A supplier with email '{email}' already exists.");

            supplier.Name = request.Name.Trim();
            supplier.Email = email;
            supplier.Phone = Normalize(request.Phone);
            supplier.Address = Normalize(request.Address);

            _supplierRepository.Update(supplier);
            await _supplierRepository.SaveChangesAsync(ct);

            return MapToDto(supplier, 0);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id, ct);
            if (supplier is null)
                return false;

            // Prevent deletion if supplier has purchase orders
            // For now, we check this by trying to delete and catching if there's a constraint violation
            // Ideally, we'd have a GetPurchaseOrderCountAsync method in the repository
            _supplierRepository.Delete(supplier);
            try
            {
                await _supplierRepository.SaveChangesAsync(ct);
            }
            catch (Exception ex) when (ex.InnerException?.Message.Contains("FOREIGN KEY constraint failed") ?? false)
            {
                throw new InvalidOperationException(
                    $"Supplier '{supplier.Name}' has purchase orders and cannot be deleted.");
            }

            return true;
        }

        private static SupplierDto MapToDto(Supplier supplier, int purchaseOrderCount) => new()
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Address = supplier.Address,
            PurchaseOrderCount = purchaseOrderCount
        };

        // Turns blank strings into null so optional fields are stored consistently.
        private static string? Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
