using System.ComponentModel.DataAnnotations;

namespace DTOs.Users
{
    public class UpdateUserRequest
    {
        // Id comes from the route. UserName is not editable here because it's the login name.

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [StringLength(200)]
        public string? FullName { get; set; }

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        // Not included: password (use a separate change-password request),
        // IsActive (use a deactivate/reactivate action), and roles (manage through role assignment).
    }
}