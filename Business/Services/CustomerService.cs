using Business.Interfaces;
using Data.Interfaces;
using DTOs.Customers;
using IMS.Entities;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken ct = default)
        {
            var customers = await _customerRepository.GetAllAsync(ct);
            var counts = await _customerRepository.GetSalesOrderCountsAsync(ct);

            return customers
                .Select(c => MapToDto(c, counts.GetValueOrDefault(c.Id)))
                .ToList();
        }

        public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var customer = await _customerRepository.GetByIdAsync(id, ct);
            if (customer is null)
                return null;

            var count = await _customerRepository.GetSalesOrderCountAsync(id, ct);
            return MapToDto(customer, count);
        }

        public async Task<IReadOnlyList<CustomerDto>> SearchAsync(string term, CancellationToken ct = default)
        {
            var customers = await _customerRepository.SearchByNameAsync(term, ct);
            var counts = await _customerRepository.GetSalesOrderCountsAsync(ct);

            return customers
                .Select(c => MapToDto(c, counts.GetValueOrDefault(c.Id)))
                .ToList();
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default)
        {
            var email = Normalize(request.Email);

            if (email is not null &&
                await _customerRepository.GetByEmailAsync(email, null, ct) is not null)
                throw new InvalidOperationException($"A customer with email '{email}' already exists.");

            var customer = new Customer
            {
                Name = request.Name.Trim(),
                Email = email,
                Phone = Normalize(request.Phone),
                Address = Normalize(request.Address)
            };

            await _customerRepository.AddAsync(customer, ct);
            await _customerRepository.SaveChangesAsync(ct);

            return MapToDto(customer, 0);
        }

        public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken ct = default)
        {
            var customer = await _customerRepository.GetByIdAsync(id, ct);
            if (customer is null)
                return null;

            var email = Normalize(request.Email);

            if (email is not null &&
                await _customerRepository.GetByEmailAsync(email, id, ct) is not null)
                throw new InvalidOperationException($"A customer with email '{email}' already exists.");

            customer.Name = request.Name.Trim();
            customer.Email = email;
            customer.Phone = Normalize(request.Phone);
            customer.Address = Normalize(request.Address);

            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync(ct);

            var count = await _customerRepository.GetSalesOrderCountAsync(id, ct);
            return MapToDto(customer, count);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var customer = await _customerRepository.GetByIdAsync(id, ct);
            if (customer is null)
                return false;

            var orderCount = await _customerRepository.GetSalesOrderCountAsync(id, ct);
            if (orderCount > 0)
                throw new InvalidOperationException(
                    $"Customer '{customer.Name}' has {orderCount} sales order(s) and cannot be deleted.");

            _customerRepository.Delete(customer);
            await _customerRepository.SaveChangesAsync(ct);

            return true;
        }

        private static CustomerDto MapToDto(Customer customer, int salesOrderCount) => new()
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            SalesOrderCount = salesOrderCount
        };

        // Turns blank strings into null so optional fields are stored consistently.
        private static string? Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}