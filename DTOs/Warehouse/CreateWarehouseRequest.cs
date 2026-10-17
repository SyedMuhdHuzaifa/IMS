using System.ComponentModel.DataAnnotations;

namespace DTOs.Warehouses
{
    public class CreateWarehouseRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z0-9\-_]+$",
            ErrorMessage = "Code may contain only letters, digits, hyphens and underscores.")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        // Not included: Id (database-generated), IsActive (new warehouses start active,
        // set by the service), and the navigation collections.
    }
}