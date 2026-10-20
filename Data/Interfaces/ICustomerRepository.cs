using IMS.Entities;

namespace Data.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        // Pass excludeId when updating, so a customer's own email doesn't count as a duplicate.
        Task<Customer?> GetByEmailAsync(string email, int? excludeId = null, CancellationToken ct = default);

        Task<IReadOnlyList<Customer>> SearchByNameAsync(string term, CancellationToken ct = default);

        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

        Task<int> GetSalesOrderCountAsync(int customerId, CancellationToken ct = default);

        // One query for the whole list. Keys are customer IDs. Customers with no orders are absent.
        Task<IReadOnlyDictionary<int, int>> GetSalesOrderCountsAsync(CancellationToken ct = default);
    }
}