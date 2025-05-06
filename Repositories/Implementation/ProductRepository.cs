using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.Repositories.Implementation
{
    // This class implements the IProductRepository interface and provides the actual logic
    // for interacting with the Product data in the database.
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        // Constructor that accepts the AppDbContext to interact with the database.
        // Dependency injection is used to provide the context.
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // Fetches all products from the database, including their associated Farmer and User details.
        // Returns a list of Product objects.
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Farmer) // Includes the related Farmer entity.
                .ThenInclude(f => f.User) // Includes the related User entity of the Farmer.
                .ToListAsync(); // Converts the result to a list asynchronously.
        }

        // Fetches all products associated with a specific farmer by their FarmerId.
        // Orders the products by their production date in descending order.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        // Returns a list of Product objects belonging to the specified farmer.
        public async Task<IEnumerable<Product>> GetProductsByFarmerIdAsync(int farmerId)
        {
            return await _context.Products
                .Where(p => p.FarmerId == farmerId) // Filters products by FarmerId.
                .OrderByDescending(p => p.ProductionDate) // Orders by production date (newest first).
                .ToListAsync(); // Converts the result to a list asynchronously.
        }

        // Fetches all products associated with a specific farmer within a given date range.
        // Orders the products by their production date in descending order.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        //   startDate - The start date of the range.
        //   endDate - The end date of the range.
        // Returns a list of Product objects that were produced within the specified date range.
        public async Task<IEnumerable<Product>> GetProductsByDateRangeAsync(int farmerId, DateTime startDate, DateTime endDate)
        {
            return await _context.Products
                .Where(p => p.FarmerId == farmerId && p.ProductionDate >= startDate && p.ProductionDate <= endDate) // Filters by FarmerId and date range.
                .OrderByDescending(p => p.ProductionDate) // Orders by production date (newest first).
                .ToListAsync(); // Converts the result to a list asynchronously.
        }

        // Fetches all products associated with a specific farmer that belong to a specific category.
        // Orders the products by their production date in descending order.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        //   category - The category of the products (e.g., "Dairy", "Vegetables").
        // Returns a list of Product objects that match the specified category.
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int farmerId, string category)
        {
            return await _context.Products
                .Where(p => p.FarmerId == farmerId && p.Category == category) // Filters by FarmerId and category.
                .OrderByDescending(p => p.ProductionDate) // Orders by production date (newest first).
                .ToListAsync(); // Converts the result to a list asynchronously.
        }

        // Finds a specific product by its unique ID (ProductId).
        // Includes the associated Farmer details.
        // Parameters:
        //   id - The ID of the product you’re looking for.
        // Returns the Product object if found, or null if it doesn’t exist.
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Farmer) // Includes the related Farmer entity.
                .FirstOrDefaultAsync(p => p.ProductId == id); // Finds the first match or returns null.
        }

        // Adds a new product to the database.
        // Parameters:
        //   product - The Product object containing the details of the new product.
        // Saves the product to the database asynchronously.
        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product); // Adds the product to the context.
            await _context.SaveChangesAsync(); // Saves the changes to the database.
        }

        // Updates an existing product’s details in the database.
        // Parameters:
        //   product - The Product object with the updated information.
        // Saves the changes to the database asynchronously.
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product); // Marks the product entity as updated.
            await _context.SaveChangesAsync(); // Saves the changes to the database.
        }

        // Checks if a product exists in the database by its unique ID (ProductId).
        // Parameters:
        //   id - The ID of the product you want to check.
        // Returns true if the product exists, or false if it doesn’t.
        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _context.Products
                .AnyAsync(p => p.ProductId == id); // Checks if any product matches the given ID.
        }
    }
}