using Business.Interfaces;
using Data.Interfaces;
using DTOs.Users;
using IMS.Entities;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IApplicationUserRepositery _userRepository;
        private readonly IApplicationRoleRepository _roleRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserService(
            IApplicationUserRepositery userRepository,
            IApplicationRoleRepository roleRepository,
            UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
        }

        public async Task<IReadOnlyList<UserDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
        {
            var users = await _userRepository.GetAllAsync(includeInactive, ct);

            var result = new List<UserDto>(users.Count);
            foreach (var user in users)
                result.Add(await MapToDtoAsync(user));

            return result;
        }

        public async Task<UserDto?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            return user is null ? null : await MapToDtoAsync(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
        {
            var userName = request.UserName.Trim();
            var email = request.Email.Trim();

            if (await _userRepository.GetByUserNameAsync(userName, ct) is not null)
                throw new InvalidOperationException($"A user named '{userName}' already exists.");

            if (await _userRepository.GetByEmailAsync(email, ct) is not null)
                throw new InvalidOperationException($"A user with email '{email}' already exists.");

            // Check roles before creating the user so a bad role name doesn't leave a user without roles.
            foreach (var role in request.Roles)
            {
                if (!await _roleRepository.ExistsAsync(role, ct))
                    throw new InvalidOperationException($"Role '{role}' does not exist.");
            }

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                FullName = request.FullName?.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            EnsureSucceeded(await _userManager.CreateAsync(user, request.Password));

            if (request.Roles.Count > 0)
                EnsureSucceeded(await _userManager.AddToRolesAsync(user, request.Roles.Distinct(StringComparer.OrdinalIgnoreCase)));

            return await MapToDtoAsync(user);
        }

        public async Task<UserDto?> UpdateAsync(string id, UpdateUserRequest request, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user is null)
                return null;

            var email = request.Email.Trim();

            // Allow the user to keep their own email, but not take someone else's.
            var emailOwner = await _userRepository.GetByEmailAsync(email, ct);
            if (emailOwner is not null && emailOwner.Id != user.Id)
                throw new InvalidOperationException($"A user with email '{email}' already exists.");

            user.Email = email;
            user.NormalizedEmail = _userManager.NormalizeEmail(email);
            user.FullName = request.FullName?.Trim();
            user.PhoneNumber = request.PhoneNumber?.Trim();

            // UserName is not editable here, so it's left alone.
            EnsureSucceeded(await _userManager.UpdateAsync(user));

            return await MapToDtoAsync(user);
        }

        public async Task<UserDto?> SetRolesAsync(string id, IReadOnlyList<string> roleNames, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user is null)
                return null;

            var requested = roleNames
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var role in requested)
            {
                if (!await _roleRepository.ExistsAsync(role, ct))
                    throw new InvalidOperationException($"Role '{role}' does not exist.");
            }

            var current = await _userManager.GetRolesAsync(user);

            var toRemove = current.Except(requested, StringComparer.OrdinalIgnoreCase).ToList();
            var toAdd = requested.Except(current, StringComparer.OrdinalIgnoreCase).ToList();

            if (toRemove.Count > 0)
                EnsureSucceeded(await _userManager.RemoveFromRolesAsync(user, toRemove));

            if (toAdd.Count > 0)
                EnsureSucceeded(await _userManager.AddToRolesAsync(user, toAdd));

            return await MapToDtoAsync(user);
        }

        public async Task<bool> DeactivateAsync(string id, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user is null)
                return false;

            if (!user.IsActive)
                return true;

            user.IsActive = false;
            EnsureSucceeded(await _userManager.UpdateAsync(user));

            // Changing the security stamp invalidates existing login cookies and tokens.
            await _userManager.UpdateSecurityStampAsync(user);

            return true;
        }

        public async Task<bool> ReactivateAsync(string id, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            if (user is null)
                return false;

            if (user.IsActive)
                return true;

            user.IsActive = true;
            EnsureSucceeded(await _userManager.UpdateAsync(user));

            return true;
        }

        private async Task<UserDto> MapToDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles.OrderBy(r => r).ToList()
            };
        }

        // Identity reports failures (weak password, duplicate key, invalid email) through IdentityResult.
        private static void EnsureSucceeded(IdentityResult result)
        {
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    string.Join(" ", result.Errors.Select(e => e.Description)));
        }
    }
}