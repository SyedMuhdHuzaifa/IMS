using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IStockTransactionRepository: IGenericRepository<StockTransaction>
    {
        Task<IReadOnlyList<StockTransaction>> GetByProductAsync(int productId, int? warehouseId = null, CancellationToken ct = default);
        Task<IReadOnlyList<StockTransaction>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default);
        Task<IReadOnlyList<StockTransaction>> GetByReferenceAsync(string transactionType, int referenceId, CancellationToken ct = default);
        Task<IReadOnlyList<StockTransaction>> GetByDateRangeAsync(DateTime from, DateTime to, int? warehouseId = null, CancellationToken ct = default);
        Task<int> GetNetQuantityAsync(int productId, int warehouseId, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<StockTransaction> transactions, CancellationToken ct = default);
    }
}
