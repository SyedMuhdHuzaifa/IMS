using System.ComponentModel.DataAnnotations;

namespace DTOs.Categories
{
    public class CreateCategoryRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        // Not included: Id (database-generated) and Products (managed through Product).
    }
}