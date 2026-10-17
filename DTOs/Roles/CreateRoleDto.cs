using System.ComponentModel.DataAnnotations;

namespace Dtos.Roles
{
    public class CreateRoleRequest
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}