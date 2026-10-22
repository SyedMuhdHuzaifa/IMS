using Business.Interfaces;
using Data.Interfaces;
using DTOs.Warehouses;
using IMS.Entities;

namespace Business.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseStockRepository _warehouseStockRepository;

        public WarehouseService(
            IWarehouseRepository warehouseRepository,
            IWarehouseStockRepository warehouseStockRepository)
        {
            _warehouseRepository = warehouseRepository;
            _warehouseStockRepository = warehouseStockRepository;
        }

        public async Task<IReadOnlyList<WarehouseDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
        {
            var warehouses = await _warehouseRepository.GetAllAsync(includeInactive, ct);
            return warehouses.Select(MapToDto).ToList();
        }

        public async Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id, ct);
            return warehouse is null ? null : MapToDto(warehouse);
        }

        public async Task<WarehouseDto> CreateAsync(CreateWarehouseRequest request, CancellationToken ct = default)
        {
            var code = request.Code.Trim();

            if (await _warehouseRepository.CodeExistsAsync(code, null, ct))
                throw new InvalidOperationException($"A warehouse with code '{code}' already exists.");

            var warehouse = new Warehouse
            {
                Name = request.Name.Trim(),
                Code = code,
                Location = Normalize(request.Location),
                Description = Normalize(request.Description),
                IsActive = true
            };

            await _warehouseRepository.AddAsync(warehouse, ct);
            await _warehouseRepository.SaveChangesAsync(ct);

            return MapToDto(warehouse);
        }

        public async Task<WarehouseDto?> UpdateAsync(int id, UpdateWarehouseRequest request, CancellationToken ct = default)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id, ct);
            if (warehouse is null)
                return null;

            warehouse.Name = request.Name.Trim();
            warehouse.Location = Normalize(request.Location);
            warehouse.Description = Normalize(request.Description);

            _warehouseRepository.Update(warehouse);
            await _warehouseRepository.SaveChangesAsync(ct);

            return MapToDto(warehouse);
        }

        public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id, ct);
            if (warehouse is null)
                return false;

            if (!warehouse.IsActive)
                return true;

            var stocks = await _warehouseStockRepository.GetByWarehouseAsync(id, ct);
            if (stocks.Any(s => s.QuantityOnHand > 0))
            {
                throw new InvalidOperationException(
                    $"Warehouse '{warehouse.Name}' has active stock on hand and cannot be deactivated. Transfer or adjust stock first.");
            }

            warehouse.IsActive = false;
            _warehouseRepository.Update(warehouse);
            await _warehouseRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> ReactivateAsync(int id, CancellationToken ct = default)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id, ct);
            if (warehouse is null)
                return false;

            if (warehouse.IsActive)
                return true;

            warehouse.IsActive = true;
            _warehouseRepository.Update(warehouse);
            await _warehouseRepository.SaveChangesAsync(ct);

            return true;
        }

        private static WarehouseDto MapToDto(Warehouse warehouse) => new()
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Code = warehouse.Code,
            Location = warehouse.Location,
            Description = warehouse.Description,
            IsActive = warehouse.IsActive
        };

        private static string? Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
