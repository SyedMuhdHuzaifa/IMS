using DTOs.Customers;

namespace IMS_WEB.Services.Interface
{
    public interface ICustomerApiService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken ct = default);
        Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<CustomerDto>> SearchAsync(string term, CancellationToken ct = default);
        Task<bool> CreateAsync(CreateCustomerRequest dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateCustomerRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
