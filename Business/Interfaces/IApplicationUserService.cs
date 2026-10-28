using DTOs.Users;

namespace Business.Interfaces
{
    public interface IApplicationUserService
    {
        Task<IReadOnlyList<UserDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);

        Task<UserDto?> GetByIdAsync(string id, CancellationToken ct = default);

        // Throws if the username or email is taken, a role doesn't exist, or the password fails policy.
        Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct = default);

        // Returns null if the user doesn't exist. Throws if the new email belongs to another user.
        Task<UserDto?> UpdateAsync(string id, UpdateUserRequest request, CancellationToken ct = default);

        // Replaces the user's roles. Returns null if the user doesn't exist.
        Task<UserDto?> SetRolesAsync(string id, IReadOnlyList<string> roleNames, CancellationToken ct = default);

        // Returns false if the user doesn't exist. Also signs the user out everywhere.
        Task<bool> DeactivateAsync(string id, CancellationToken ct = default);

        // Returns false if the user doesn't exist.
        Task<bool> ReactivateAsync(string id, CancellationToken ct = default);
    }
}