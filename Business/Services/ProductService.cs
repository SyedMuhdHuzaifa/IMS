using Business.Interfaces;
using Data.Interfaces;
using DTOs.Products;
using IMS.Entities;

namespace Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWarehouseStockRepository _warehouseStockRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IWarehouseStockRepository warehouseStockRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _warehouseStockRepository = warehouseStockRepository;
        }

        public async Task<IReadOnlyList<ProductDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
        {
            var products = await _productRepository.GetAllWithCategoryAsync(includeInactive, ct);
            var totals = await _warehouseStockRepository.GetTotalsByProductAsync(ct);

            return products
                .Select(p => MapToDto(p, totals.GetValueOrDefault(p.Id)))
                .ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await _productRepository.GetByIdWithCategoryAsync(id, ct);
            if (product is null)
                return null;

            var total = await _warehouseStockRepository.GetTotalOnHandAsync(id, ct);
            return MapToDto(product, total);
        }

        public async Task<IReadOnlyList<ProductDto>> GetByCategoryAsync(int categoryId, bool includeInactive = false, CancellationToken ct = default)
        {
            var products = await _productRepository.GetByCategoryIdAsync(categoryId, includeInactive, ct);
            var totals = await _warehouseStockRepository.GetTotalsByProductAsync(ct);

            return products
                .Select(p => MapToDto(p, totals.GetValueOrDefault(p.Id)))
                .ToList();
        }

        public async Task<IReadOnlyList<ProductDto>> SearchAsync(string term, CancellationToken ct = default)
        {
            var products = await _productRepository.SearchByNameAsync(term, ct);
            var totals = await _warehouseStockRepository.GetTotalsByProductAsync(ct);

            return products
                .Select(p => MapToDto(p, totals.GetValueOrDefault(p.Id)))
                .ToList();
        }

        public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
        {
            var sku = request.SKU.Trim();

            if (await _productRepository.SkuExistsAsync(sku, null, ct))
                throw new InvalidOperationException($"A product with SKU '{sku}' already exists.");

            await EnsureCategoryExistsAsync(request.CategoryId, ct);

            var product = new Product
            {
                Name = request.Name.Trim(),
                SKU = sku,
                Description = Normalize(request.Description),
                PurchasePrice = request.PurchasePrice,
                SellingPrice = request.SellingPrice,
                ReorderLevel = request.ReorderLevel,
                CategoryId = request.CategoryId,
                IsActive = true
            };

            await _productRepository.AddAsync(product, ct);
            await _productRepository.SaveChangesAsync(ct);

            // A new product has no stock rows yet, so its total is 0.
            var saved = await _productRepository.GetByIdWithCategoryAsync(product.Id, ct);
            return MapToDto(saved!, 0);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductRequest request, CancellationToken ct = default)
        {
            var product = await _productRepository.GetByIdAsync(id, ct);
            if (product is null)
                return null;

            await EnsureCategoryExistsAsync(request.CategoryId, ct);

            // SKU is intentionally not changed here. See the note on UpdateProductRequest.
            product.Name = request.Name.Trim();
            product.Description = Normalize(request.Description);
            product.PurchasePrice = request.PurchasePrice;
            product.SellingPrice = request.SellingPrice;
            product.ReorderLevel = request.ReorderLevel;
            product.CategoryId = request.CategoryId;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(ct);

            var updated = await _productRepository.GetByIdWithCategoryAsync(id, ct);
            var total = await _warehouseStockRepository.GetTotalOnHandAsync(id, ct);
            return MapToDto(updated!, total);
        }

        public async Task<bool> DeactivateAsync(int id, CancellationToken ct = default)
        {
            var product = await _productRepository.GetByIdAsync(id, ct);
            if (product is null)
                return false;

            if (!product.IsActive)
                return true;

            product.IsActive = false;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> ReactivateAsync(int id, CancellationToken ct = default)
        {
            var product = await _productRepository.GetByIdAsync(id, ct);
            if (product is null)
                return false;

            if (product.IsActive)
                return true;

            product.IsActive = true;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task EnsureCategoryExistsAsync(int categoryId, CancellationToken ct)
        {
            if (!await _categoryRepository.ExistsAsync(categoryId, ct))
                throw new InvalidOperationException($"Category '{categoryId}' does not exist.");
        }

        private static ProductDto MapToDto(Product product, int totalOnHand) => new()
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            PurchasePrice = product.PurchasePrice,
            SellingPrice = product.SellingPrice,
            ReorderLevel = product.ReorderLevel,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            TotalOnHand = totalOnHand,
            IsLowStock = product.IsActive && totalOnHand <= product.ReorderLevel
        };

        private static string? Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}