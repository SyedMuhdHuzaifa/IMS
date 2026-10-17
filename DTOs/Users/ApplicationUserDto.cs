namespace DTOs.Users
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Not on the entity. The service fills this from UserManager.GetRolesAsync.
        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    }
}