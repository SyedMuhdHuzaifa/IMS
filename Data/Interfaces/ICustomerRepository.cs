using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface ICustomerRepository :IGenericRepository<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IReadOnlyList<Customer>> SearchByNameAsync(string term, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
