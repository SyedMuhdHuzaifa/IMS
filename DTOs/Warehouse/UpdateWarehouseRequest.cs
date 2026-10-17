using System.ComponentModel.DataAnnotations;

namespace DTOs.Warehouses
{
    // Code is not editable, for the same reason as SKU on products.
    public class UpdateWarehouseRequest
    {
        // Id comes from the route.

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}