using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Helpers;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Repositories.Interfaces;
using AgriEnergyConnect.Services.Interfaces;

namespace AgriEnergyConnect.Services.Implementation
{
    // The ProductService class implements the IProductService interface and provides the business logic
    // for managing products in the system. It interacts with the repository layer to perform CRUD operations
    // and applies additional filtering and mapping logic as needed.
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        // Constructor that accepts the IProductRepository to interact with the database.
        // Dependency injection is used to provide the repository.
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Retrieves all products from the database.
        // Returns a collection of Product objects.
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        // Retrieves all products from the database and maps them to ProductDTOs.
        // Returns a collection of ProductDTO objects containing detailed product information.
        public async Task<IEnumerable<ProductDTO>> GetAllProductDTOsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return MappingHelper.ToProductDTOList(products);
        }

        // Retrieves all products associated with a specific farmer by their FarmerId.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        // Returns a collection of Product objects belonging to the specified farmer.
        public async Task<IEnumerable<Product>> GetProductsByFarmerIdAsync(int farmerId)
        {
            return await _productRepository.GetProductsByFarmerIdAsync(farmerId);
        }

        // Retrieves all products associated with a specific farmer by their FarmerId and maps them to ProductDTOs.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        // Returns a collection of ProductDTO objects containing detailed product information.
        public async Task<IEnumerable<ProductDTO>> GetProductDTOsByFarmerIdAsync(int farmerId)
        {
            var products = await _productRepository.GetProductsByFarmerIdAsync(farmerId);
            return MappingHelper.ToProductDTOList(products);
        }

        // Retrieves all products associated with a specific farmer that match the given filter criteria.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        //   startDate - The start date of the production date range (optional).
        //   endDate - The end date of the production date range (optional).
        //   category - The category of the products to filter by (optional).
        // Returns a collection of Product objects that match the filter criteria.
        public async Task<IEnumerable<Product>> GetProductsByFilterAsync(int farmerId, DateTime? startDate, DateTime? endDate, string category)
        {
            // Get all products for the farmer.
            var products = await _productRepository.GetProductsByFarmerIdAsync(farmerId);

            // Apply date range filters.
            if (startDate.HasValue && endDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate >= startDate.Value && p.ProductionDate <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate <= endDate.Value);
            }

            // Apply category filter.
            if (!string.IsNullOrWhiteSpace(category) && category != "All Categories")
            {
                products = products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            return products.ToList();
        }

        // Retrieves all products that match the given filter criteria and maps them to ProductDTOs.
        // Parameters:
        //   filter - A ProductFilterDTO object containing the filtering criteria.
        // Returns a collection of ProductDTO objects that match the filter criteria.
        public async Task<IEnumerable<ProductDTO>> GetFilteredProductDTOsAsync(ProductFilterDTO filter)
        {
            // Get basic products based on farmerId.
            var products = filter.FarmerId.HasValue
                ? await _productRepository.GetProductsByFarmerIdAsync(filter.FarmerId.Value)
                : await _productRepository.GetAllProductsAsync();

            // Apply date range filters.
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate >= filter.StartDate.Value && p.ProductionDate <= filter.EndDate.Value);
            }
            else if (filter.StartDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate >= filter.StartDate.Value);
            }
            else if (filter.EndDate.HasValue)
            {
                products = products.Where(p => p.ProductionDate <= filter.EndDate.Value);
            }

            // Apply category filter.
            if (!string.IsNullOrEmpty(filter.Category) && filter.Category != "All Categories")
            {
                products = products.Where(p => p.Category.Equals(filter.Category, StringComparison.OrdinalIgnoreCase));
            }

            // Apply search term filter.
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Description?.ToLower().Contains(searchTerm) == true ||
                    p.Category.ToLower().Contains(searchTerm));
            }

            // Apply active/inactive filter
            if (filter.ActiveOnly.HasValue)
            {
                products = products.Where(p => p.IsActive == filter.ActiveOnly.Value);
            }

            // Convert to DTOs.
            return MappingHelper.ToProductDTOList(products);
        }

        // Retrieves a specific product by its unique ID (ProductId).
        // Parameters:
        //   id - The ID of the product you’re looking for.
        // Returns the Product object if found, or null if it doesn’t exist.
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        // Retrieves detailed information for a specific product by its unique ID (ProductId).
        // Parameters:
        //   id - The ID of the product you’re looking for.
        // Returns a ProductDTO containing detailed information about the product, or null if it doesn’t exist.
        public async Task<ProductDTO> GetProductDTOByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            return MappingHelper.ToProductDTO(product);
        }

        // Adds a new product to the database.
        // Parameters:
        //   product - The Product object containing the details of the new product.
        // Saves the product to the database.
        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddProductAsync(product);
        }

        // Updates an existing product’s details in the database.
        // Parameters:
        //   product - The Product object with the updated information.
        // Saves the changes to the database.
        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateProductAsync(product);
        }

        // Checks if a product exists in the database by its unique ID (ProductId).
        // Parameters:
        //   id - The ID of the product you want to check.
        // Returns true if the product exists, or false if it doesn’t.
        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _productRepository.ProductExistsAsync(id);
        }

        // Retrieves all unique product categories from the database.
        // Returns a collection of strings representing the available product categories.
        public async Task<IEnumerable<string>> GetAllCategoriesAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Select(p => p.Category).Distinct().ToList();
        }

        public async Task<(IEnumerable<ProductDTO> Products, List<string> Categories)> GetFarmerProductsForViewAsync(int farmerId, string searchTerm = "", string category = "", string statusFilter = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            // Create filter DTO
            var filter = new ProductFilterDTO
            {
                FarmerId = farmerId,
                SearchTerm = searchTerm,
                Category = category,
                StartDate = startDate,
                EndDate = endDate
            };

            // Apply active status filter if provided
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "active")
                {
                    filter.ActiveOnly = true;
                }
                else if (statusFilter == "inactive")
                {
                    filter.ActiveOnly = false;
                }
                // If statusFilter is empty or "all", don't set ActiveOnly (show all)
            }

            // Get products with filter
            var products = await GetFilteredProductDTOsAsync(filter);

            // Get all available categories for this farmer's products
            var allCategories = products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            return (products, allCategories);
        }

        public async Task<(List<ProductDTO> Products, List<string> Categories, int TotalProducts, int TotalPages)> GetProductsForAdminViewAsync(string searchTerm = "", string category = "", string farmerFilter = "", string statusFilter = "", DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10)
        {
            // Create filter DTO
            var filter = new ProductFilterDTO
            {
                SearchTerm = searchTerm,
                Category = category,
                StartDate = startDate,
                EndDate = endDate
            };

            // Apply farmer filter if provided
            int? farmerIdFilter = null;
            if (!string.IsNullOrEmpty(farmerFilter) && int.TryParse(farmerFilter, out int parsedId))
            {
                farmerIdFilter = parsedId;
                filter.FarmerId = farmerIdFilter;
            }

            // Apply active status filter if provided
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "active")
                {
                    filter.ActiveOnly = true;
                }
                else if (statusFilter == "inactive")
                {
                    filter.ActiveOnly = false;
                }
                // If statusFilter is empty or "all", don't set ActiveOnly (show all)
            }

            // Get products with filter
            var allProducts = await GetFilteredProductDTOsAsync(filter);

            // Get available categories for all products
            var allCategories = await GetAllCategoriesAsync();

            // Apply pagination
            var totalProducts = allProducts.Count();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            var paginatedProducts = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (paginatedProducts, allCategories.ToList(), totalProducts, totalPages);
        }

        public async Task<(ProductDTO Product, Farmer AssociatedFarmer)> GetProductDetailsForViewAsync(int productId)
        {
            // Get the product details with associated farmer information
            var product = await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found");

            // Check if Farmer is loaded, if not load it
            if (product.Farmer == null)
            {
                throw new InvalidOperationException("Product's farmer information unavailable");
            }

            // Convert to DTO
            var productDTO = MappingHelper.ToProductDTO(product);

            // Return both the product DTO and the farmer entity
            return (productDTO, product.Farmer);
        }

        public async Task DeactivateProductAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found");

            // Only update if necessary
            if (product.IsActive)
            {
                product.IsActive = false;
                await _productRepository.UpdateProductAsync(product);
            }
        }

        public async Task ActivateProductAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found");

            // Only update if necessary
            if (!product.IsActive)
            {
                product.IsActive = true;
                await _productRepository.UpdateProductAsync(product);
            }
        }
    }
}