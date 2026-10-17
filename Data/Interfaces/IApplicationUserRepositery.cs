using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IApplicationUserRepositery
    {
        Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken ct = default);
        Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IReadOnlyList<ApplicationUser>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
        Task<bool> ExistsAsync(string id, CancellationToken ct = default);
        Task AddAsync(ApplicationUser user, CancellationToken ct = default);
        void Update(ApplicationUser user);
        Task DeactivateAsync(string id, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
