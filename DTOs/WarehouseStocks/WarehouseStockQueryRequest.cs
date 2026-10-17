using System.ComponentModel.DataAnnotations;

namespace DTOs.WarehouseStocks
{
    // Query-string filter for GET /warehouse-stock.
    public class WarehouseStockQueryRequest
    {
        public int? WarehouseId { get; set; }
        public int? ProductId { get; set; }

        // When true, rows with QuantityOnHand = 0 are hidden.
        public bool ExcludeZero { get; set; } = false;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 200)]
        public int PageSize { get; set; } = 50;

        // Not included: sorting. Results are ordered by warehouse code, then product name.
    }
}