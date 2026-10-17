namespace IMS.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
        public ICollection<StockTransfer> SourceStockTransfers { get; set; } = new List<StockTransfer>();
        public ICollection<StockTransfer> DestinationStockTransfers { get; set; } = new List<StockTransfer>();
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    }
}