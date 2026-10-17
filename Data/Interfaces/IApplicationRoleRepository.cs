using System;
using System.Collections.Generic;
using System.Text;
using IMS.Entities;
using Microsoft.AspNetCore.Identity;
namespace Data.Interfaces
{
    public interface IApplicationRoleRepository
    {
        Task<ApplicationRole?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<ApplicationRole?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ApplicationRole>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string roleName, CancellationToken cancellationToken = default);
        Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken = default);
        Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken = default);
        Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default);
    }
}
