using DTOs.PurchaseOrders;

namespace Business.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<IReadOnlyList<PurchaseOrderDto>> GetAllAsync(CancellationToken ct = default);

        Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<PurchaseOrderDto>> GetBySupplierAsync(int supplierId, CancellationToken ct = default);

        Task<IReadOnlyList<PurchaseOrderDto>> GetByDestinationWarehouseAsync(int warehouseId, CancellationToken ct = default);

        Task<IReadOnlyList<PurchaseOrderDto>> GetByStatusAsync(string status, CancellationToken ct = default);

        // Throws if supplier or warehouse doesn't exist, or items are missing/invalid.
        Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderRequest request, string userId, CancellationToken ct = default);

        // Returns null if the order doesn't exist. Throws if not in Draft status.
        Task<PurchaseOrderDto?> UpdateAsync(int id, UpdatePurchaseOrderRequest request, CancellationToken ct = default);

        // Returns false if the order doesn't exist. Throws if order is not in Draft status.
        Task<bool> SubmitAsync(int id, CancellationToken ct = default);

        // Returns false if the order doesn't exist. Throws if order is not in Submitted status.
        Task<bool> ReceiveAsync(int id, CancellationToken ct = default);

        // Returns false if the order doesn't exist. Throws if order is not in Draft or Submitted.
        Task<bool> CancelAsync(int id, CancellationToken ct = default);
    }
}
