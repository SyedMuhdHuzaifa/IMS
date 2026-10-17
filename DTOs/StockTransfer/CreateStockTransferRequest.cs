using System.ComponentModel.DataAnnotations;

namespace DTOs.StockTransfers
{
    public class CreateStockTransferRequest : IValidatableObject
    {
        [Range(1, int.MaxValue)]
        public int SourceWarehouseId { get; set; }

        [Range(1, int.MaxValue)]
        public int DestinationWarehouseId { get; set; }

        // Defaults to today on the server if omitted.
        public DateTime? TransferDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required]
        [MinLength(1)]
        public IReadOnlyList<StockTransferItemRequest> Items { get; set; } = Array.Empty<StockTransferItemRequest>();

        // Not included: Id, Status (new transfers start as Draft), CompletedAt,
        // ApprovedByUserId, and CreatedByUserId (taken from the authenticated user).

        // Rules that depend on the whole request, so attributes can't express them.
        // These are pure input checks, so they run before any database call.
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