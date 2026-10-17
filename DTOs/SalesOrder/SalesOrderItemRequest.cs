using System.ComponentModel.DataAnnotations;

namespace DTOs.SalesOrders
{
    // Used by create and update. Quantity is checked against available stock in the service.
    public class SalesOrderItemRequest
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Optional. If omitted, the service uses the product's current SellingPrice.
        // The chosen price is then saved on the item, so later price changes don't affect this order.
        [Range(0, 999999999.99)]
        public decimal? UnitPrice { get; set; }

        // Not included: Id and SalesOrderId (set from the parent order).
    }
}