using DTOs.SalesOrders;

namespace IMS_WEB.Services.Interface
{
    public interface ISalesOrderApiService
    {
        Task<IEnumerable<SalesOrderSummaryDto>> GetAllAsync(CancellationToken ct = default);
        Task<SalesOrderDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<SalesOrderSummaryDto>> GetByCustomerAsync(int customerId, CancellationToken ct = default);
        Task<IEnumerable<SalesOrderSummaryDto>> GetByStatusAsync(string status, CancellationToken ct = default);
        Task<SalesOrderDto?> CreateAsync(CreateSalesOrderRequest request, CancellationToken ct = default);
        Task<SalesOrderDto?> UpdateAsync(int id, UpdateSalesOrderRequest request, CancellationToken ct = default);
        Task<bool> SubmitAsync(int id, CancellationToken ct = default);
        Task<bool> ShipAsync(int id, CancellationToken ct = default);
        Task<bool> CancelAsync(int id, CancellationToken ct = default);
    }
}
