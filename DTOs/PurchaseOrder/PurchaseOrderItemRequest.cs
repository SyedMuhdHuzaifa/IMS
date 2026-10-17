using System.ComponentModel.DataAnnotations;

namespace DTOs.PurchaseOrders
{
    // Used by both create and update. Shared so the item rules are defined once.
    public class PurchaseOrderItemRequest
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, 999999999.99)]
        public decimal UnitPrice { get; set; }

        // Not included: Id (items are identified by their position in the order)
        // and PurchaseOrderId (set from the parent order).
    }
}