using System.ComponentModel.DataAnnotations;

namespace DTOs.Products
{
    public class UpdateProductRequest
    {
        // Id comes from the route. SKU is not editable here (see notes).

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0, 999999999.99)]
        public decimal PurchasePrice { get; set; }

        [Range(0, 999999999.99)]
        public decimal SellingPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}