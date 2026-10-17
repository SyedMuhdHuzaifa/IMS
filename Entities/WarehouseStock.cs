namespace IMS.Entities
{
    public class WarehouseStock
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int QuantityOnHand { get; set; }

        // Concurrency token (SQL Server rowversion). Prevents lost updates
        // when two transactions change the same stock row at once.
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // Navigation properties
        public Warehouse? Warehouse { get; set; }
        public Product? Product { get; set; }
    }
}
