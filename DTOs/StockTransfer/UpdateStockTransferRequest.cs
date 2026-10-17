using System.ComponentModel.DataAnnotations;

namespace DTOs.StockTransfers
{
    // Allowed only while the transfer is in Draft.
    public class UpdateStockTransferRequest : IValidatableObject
    {
        // Id comes from the route.

        [Range(1, int.MaxValue)]
        public int SourceWarehouseId { get; set; }

        [Range(1, int.MaxValue)]
        public int DestinationWarehouseId { get; set; }

        public DateTime TransferDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        // The full list replaces the existing items.
        [Required]
        [MinLength(1)]
        public IReadOnlyList<StockTransferItemRequest> Items { get; set; } = Array.Empty<StockTransferItemRequest>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SourceWarehouseId == DestinationWarehouseId)
                yield return new ValidationResult(
                    "Source and destination warehouses must be different.",
                    new[] { nameof(DestinationWarehouseId) });

            if (Items.GroupBy(i => i.ProductId).Any(g => g.Count() > 1))
                yield return new ValidationResult(
                    "Each product can appear only once per transfer. Combine the quantities into one line.",
                    new[] { nameof(Items) });
        }
    }
}