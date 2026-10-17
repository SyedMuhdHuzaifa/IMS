using System.ComponentModel.DataAnnotations;

namespace DTOs.PurchaseOrders
{
    // Allowed only while the order is in Draft. The service rejects it otherwise.
    public class UpdatePurchaseOrderRequest
    {
        // Id comes from the route.

        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }

        [Range(1, int.MaxValue)]
        public int DestinationWarehouseId { get; set; }

        public DateTime OrderDate { get; set; }

        // The full list replaces the existing items. Partial item edits are harder
        // to get right and to audit, so this keeps the rule simple.
        [Required]
        [MinLength(1)]
        public IReadOnlyList<PurchaseOrderItemRequest> Items { get; set; } = Array.Empty<PurchaseOrderItemRequest>();

        // Not included: Status (changed through submit, receive, and cancel actions)
        // and TotalAmount (computed).
    }
}