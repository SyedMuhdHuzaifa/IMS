using System.ComponentModel.DataAnnotations;

namespace DTOs.Categories
{
    public class UpdateCategoryRequest
    {
        // Id comes from the route.

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}