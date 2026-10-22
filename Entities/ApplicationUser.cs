using Microsoft.AspNetCore.Identity;

namespace IMS.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<PurchaseOrder> CreatedPurchaseOrders { get; set; } = new List<PurchaseOrder>();
        public ICollection<SalesOrder> CreatedSalesOrders { get; set; } = new List<SalesOrder>();
        public ICollection<StockTransaction> CreatedStockTransactions { get; set; } = new List<StockTransaction>();
        public ICollection<StockTransfer> CreatedStockTransfers { get; set; } = new List<StockTransfer>();
        public ICollection<StockTransfer> ApprovedStockTransfers { get; set; } = new List<StockTransfer>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}