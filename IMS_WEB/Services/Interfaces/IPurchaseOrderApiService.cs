using DTOs.PurchaseOrders;

namespace IMS_WEB.Services.Interface
{
    public interface IPurchaseOrderApiService
    {
        Task<IEnumerable<PurchaseOrderDto>> GetAllAsync(CancellationToken ct = default);
        Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderDto>> GetBySupplierAsync(int supplierId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderDto>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderDto>> GetByStatusAsync(string status, CancellationToken ct = default);

        Task<PurchaseOrderDto?> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken ct = default);
        Task<PurchaseOrderDto?> UpdateAsync(int id, UpdatePurchaseOrderRequest request, CancellationToken ct = default);

        Task<bool> SubmitAsync(int id, CancellationToken ct = default);
        Task<bool> ReceiveAsync(int id, CancellationToken ct = default);
        Task<bool> CancelAsync(int id, CancellationToken ct = default);
    }
}
