using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ApplicationUserRepository: IApplicationUserRepositery
    {
        private readonly AppDbContext _context;

        public ApplicationUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(
        string id,
        CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Id == id, ct);
        }

        public async Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            string normalizedUserName = userName.ToUpperInvariant();
            return await _context.Users
            .FirstOrDefaultAsync(
                user => user.NormalizedUserName == normalizedUserName,
                cancellationToken);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct =default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            string normalizedEmail = email.ToUpperInvariant();

            return await _context.Users.FirstOrDefaultAsync(user => user.Email == email, ct);
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
        {
            IQueryable<ApplicationUser> query = _context.Users;

            if (!includeInactive)
            {
                query = query.Where(user => user.IsActive);
            }

            return await query
                .OrderBy(user => user.UserName)
                .ToListAsync(ct);
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(user => user.Id == id);
        }

        public async Task AddAsync(ApplicationUser user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public void Update(ApplicationUser user)
        {
            _context.Users.Update(user);
        }

        public async Task DeactivateAsync(string id, CancellationToken ct = default)
        {
            var user = await _context.Users
            .FirstOrDefaultAsync(user => user.Id == id, ct);

            if (user is null)
            {
                throw new KeyNotFoundException(
                    $"User with ID '{id}' was not found.");
            }

            user.IsActive = false;
        }


        public async Task<int> SaveChangesAsync(
            CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }


    }
}
