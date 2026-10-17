using System.ComponentModel.DataAnnotations;

namespace DTOs.Users
{
    public class CreateUserRequest
    {
        [Required]
        [StringLength(256)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [StringLength(200)]
        public string? FullName { get; set; }

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        // Role names to assign on creation, for example "Admin" or "Storekeeper".
        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();

        // Not included: Id (server-generated), IsActive (new users start active),
        // CreatedAt (server time), and any password hash or security stamp fields.
    }
}