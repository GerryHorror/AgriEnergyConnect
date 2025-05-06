using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Repositories.Interfaces
{
    // This interface defines the methods for working with Product data.
    // It’s like a contract that ensures any class implementing it will provide these functionalities.
    public interface IProductRepository
    {
        // Fetches all the products from the database.
        // Returns a list of Product objects.
        Task<IEnumerable<Product>> GetAllProductsAsync();

        // Fetches all the products associated with a specific farmer by their FarmerId.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        // Returns a list of Product objects belonging to the specified farmer.
        Task<IEnumerable<Product>> GetProductsByFarmerIdAsync(int farmerId);

        // Fetches all the products associated with a specific farmer within a given date range.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        //   startDate - The start date of the range.
        //   endDate - The end date of the range.
        // Returns a list of Product objects that were produced within the specified date range.
        Task<IEnumerable<Product>> GetProductsByDateRangeAsync(int farmerId, DateTime startDate, DateTime endDate);

        // Fetches all the products associated with a specific farmer that belong to a specific category.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        //   category - The category of the products (e.g., "Dairy", "Vegetables").
        // Returns a list of Product objects that match the specified category.
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int farmerId, string category);

        // Finds a specific product by its unique ID (ProductId).
        // Parameters:
        //   id - The ID of the product you’re looking for.
        // Returns the Product object if found, or null if it doesn’t exist.
        Task<Product> GetProductByIdAsync(int id);

        // Adds a new product to the database.
        // Parameters:
        //   product - The Product object containing the details of the new product.
        // Doesn’t return anything, but the product will be saved in the database.
        Task AddProductAsync(Product product);

        // Updates an existing product’s details in the database.
        // Parameters:
        //   product - The Product object with the updated information.
        // Doesn’t return anything, but the changes will be saved in the database.
        Task UpdateProductAsync(Product product);

        // Checks if a product exists in the database by its unique ID (ProductId).
        // Parameters:
        //   id - The ID of the product you want to check.
        // Returns true if the product exists, or false if it doesn’t.
        Task<bool> ProductExistsAsync(int id);
    }
}