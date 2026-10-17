using System.ComponentModel.DataAnnotations;

namespace DTOs.Products
{
    public class CreateProductRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z0-9\-_.]+$",
            ErrorMessage = "SKU may contain only letters, digits, hyphens, underscores and dots.")]
        public string SKU { get; set; } = string.Empty;

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

        // Not included: Id (database-generated), IsActive (new products start active,
        // set by the service), and any stock fields (stock is added through
        // stock receipts and transfers, not on product creation).
    }
}