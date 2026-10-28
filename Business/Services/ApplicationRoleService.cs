using Data.Interfaces;
using Dtos.Roles;
using IMS.Entities;
using Microsoft.AspNetCore.Identity;
using Business.Interfaces;

namespace Business.Services
{
    public class ApplicationRoleService : IApplicationRoleService
    {
        private readonly IApplicationRoleRepository _roleRepository;

        public ApplicationRoleService(IApplicationRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken ct = default)
        {
            var roles = await _roleRepository.GetAllAsync(ct);
            return roles.Select(MapToDto).ToList();
        }

        public async Task<RoleDto?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var role = await _roleRepository.GetByIdAsync(id, ct);
            return role is null ? null : MapToDto(role);
        }

        public async Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default)
        {
            var name = request.Name.Trim();

            if (await _roleRepository.ExistsAsync(name, ct))
                throw new InvalidOperationException($"A role named '{name}' already exists.");

            var role = new ApplicationRole
            {
                // IdentityRole uses a string key that isn't generated automatically here.
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = request.Description?.Trim()
            };

            EnsureSucceeded(await _roleRepository.CreateAsync(role, ct));

            return MapToDto(role);
        }

        public async Task<RoleDto?> UpdateAsync(string id, UpdateRoleRequest request, CancellationToken ct = default)
        {
            var role = await _roleRepository.GetByIdAsync(id, ct);
            if (role is null)
                return null;

            var name = request.Name.Trim();

            // Allow the role to keep its own name, but not take another role's name.
            var roleWithSameName = await _roleRepository.GetByNameAsync(name, ct);
            if (roleWithSameName is not null && roleWithSameName.Id != role.Id)
                throw new InvalidOperationException($"A role named '{name}' already exists.");

            role.Name = name;
            role.Description = request.Description?.Trim();

            EnsureSucceeded(await _roleRepository.UpdateAsync(role, ct));

            return MapToDto(role);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
        {
            var role = await _roleRepository.GetByIdAsync(id, ct);
            if (role is null)
                return false;

            var usersInRole = await _roleRepository.GetUsersInRoleAsync(role.Name!, ct);
            if (usersInRole.Count > 0)
                throw new InvalidOperationException(
                    $"Role '{role.Name}' is still assigned to {usersInRole.Count} user(s) and cannot be deleted.");

            EnsureSucceeded(await _roleRepository.DeleteAsync(role, ct));

            return true;
        }

        private static RoleDto MapToDto(ApplicationRole role) => new()
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description
        };

        // Identity reports failures (such as a duplicate name hitting the unique index)
        // through IdentityResult, so turn those into exceptions here.
        private static void EnsureSucceeded(IdentityResult result)
        {
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    string.Join(" ", result.Errors.Select(e => e.Description)));
        }
    }
}