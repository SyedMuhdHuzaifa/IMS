using System.ComponentModel.DataAnnotations;

namespace DTOs.PurchaseOrders
{
    public class CreatePurchaseOrderRequest
    {
        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }

        [Range(1, int.MaxValue)]
        public int DestinationWarehouseId { get; set; }

        // Date the supplier order is placed. Defaults to today on the server if omitted.
        public DateTime? OrderDate { get; set; }

        [Required]
        [MinLength(1)]
        public IReadOnlyList<PurchaseOrderItemRequest> Items { get; set; } = Array.Empty<PurchaseOrderItemRequest>();

        // Not included: Id, Status (new orders start as Draft), TotalAmount (computed),
        // and CreatedByUserId (taken from the authenticated user).
    }
}