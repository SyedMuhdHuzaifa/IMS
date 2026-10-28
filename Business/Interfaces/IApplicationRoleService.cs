using Dtos.Roles;

namespace Business.Interfaces
{
    public interface IApplicationRoleService
    {
        Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken ct = default);

        Task<RoleDto?> GetByIdAsync(string id, CancellationToken ct = default);

        // Throws if a role with the same name already exists.
        Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default);

        // Returns null if the role doesn't exist. Throws if the new name is taken by another role.
        Task<RoleDto?> UpdateAsync(string id, UpdateRoleRequest request, CancellationToken ct = default);

        // Returns false if the role doesn't exist. Throws if users still hold the role.
        Task<bool> DeleteAsync(string id, CancellationToken ct = default);
    }
}