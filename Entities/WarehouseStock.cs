namespace IMS.Entities
{
    public class WarehouseStock
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int QuantityOnHand { get; set; }

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public Warehouse? Warehouse { get; set; }
        public Product? Product { get; set; }
    }
}
