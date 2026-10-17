
using Data.Interfaces;
using IMS.Data;
using IMS.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ApplicationRoleRepository : IApplicationRoleRepository
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationRoleRepository(AppDbContext context,RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<ApplicationRole?> GetByIdAsync(string id,CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(
                    role => role.Id == id,
                    cancellationToken);
        }

        public async Task<ApplicationRole?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            string normalizedName = _roleManager.NormalizeKey(roleName);

            return await _context.Roles
                .FirstOrDefaultAsync(
                    role => role.NormalizedName == normalizedName,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<ApplicationRole>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .OrderBy(role => role.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(string roleName,CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            string normalizedName = _roleManager.NormalizeKey(roleName);

            return await _context.Roles
                .AnyAsync(
                    role => role.NormalizedName == normalizedName,
                    cancellationToken);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role,CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role,CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role,CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _roleManager.DeleteAsync(role);
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetUsersInRoleAsync(string roleName,CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(roleName))
                return Array.Empty<ApplicationUser>();
            
            var users = await _userManager.GetUsersInRoleAsync(roleName);

            return users?.ToList() ?? new List<ApplicationUser>();
        }
    }
}