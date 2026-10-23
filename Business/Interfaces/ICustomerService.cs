using DTOs.Customers;

namespace Business.Interfaces
{
    public interface ICustomerService
    {
        Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken ct = default);

        Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<CustomerDto>> SearchAsync(string term, CancellationToken ct = default);

        // Throws if another customer already uses the email.
        Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default);

        // Returns null if the customer doesn't exist. Throws if the email belongs to another customer.
        Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken ct = default);

        // Returns false if the customer doesn't exist. Throws if the customer has sales orders.
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}