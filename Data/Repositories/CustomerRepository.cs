using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public class CustomerRepository:GenericRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext? _context;

        public CustomerRepository(AppDbContext context): base(context)
        {
            _context = context;
        }
        public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Customers.FirstOrDefaultAsync(customer => customer.Email == email); 
        }
        public async Task<IReadOnlyList<Customer>> SearchByNameAsync(string term, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Array.Empty<Customer>();

            term = term.Trim();

            return await _context.Customers
        .Where(customer => customer.Name.Contains(term))
        .OrderBy(customer => customer.Name)
        .ToListAsync(ct);
        }
        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Customers.AnyAsync(customer => customer.Id == id);
        }

       
    }
}
