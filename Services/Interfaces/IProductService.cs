using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Services.Interfaces
{
    // The IProductService interface defines the contract for managing products in the system.
    // It provides methods for retrieving, adding, updating, and filtering products.
    public interface IProductService
    {
        // Retrieves all products from the database.
        // Returns a collection of Product objects.
        Task<IEnumerable<Product>> GetAllProductsAsync();

        // Retrieves all products from the database and maps them to ProductDTOs.
        // Returns a collection of ProductDTO objects containing detailed product information.
        Task<IEnumerable<ProductDTO>> GetAllProductDTOsAsync();

        // Retrieves all products associated with a specific farmer by their FarmerId.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        // Returns a collection of Product objects belonging to the specified farmer.
        Task<IEnumerable<Product>> GetProductsByFarmerIdAsync(int farmerId);

        // Retrieves all products associated with a specific farmer by their FarmerId and maps them to ProductDTOs.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        // Returns a collection of ProductDTO objects containing detailed product information.
        Task<IEnumerable<ProductDTO>> GetProductDTOsByFarmerIdAsync(int farmerId);

        // Retrieves all products associated with a specific farmer that match the given filter criteria.
        // Parameters:
        //   farmerId - The ID of the farmer whose products you want to retrieve.
        //   startDate - The start date of the production date range (optional).
        //   endDate - The end date of the production date range (optional).
        //   category - The category of the products to filter by (optional).
        // Returns a collection of Product objects that match the filter criteria.
        Task<IEnumerable<Product>> GetProductsByFilterAsync(int farmerId, DateTime? startDate, DateTime? endDate, string category);

        // Retrieves all products that match the given filter criteria and maps them to ProductDTOs.
        // Parameters:
        //   filter - A ProductFilterDTO object containing the filtering criteria.
        // Returns a collection of ProductDTO objects that match the filter criteria.
        Task<IEnumerable<ProductDTO>> GetFilteredProductDTOsAsync(ProductFilterDTO filter);

        // Retrieves a specific product by its unique ID (ProductId).
        // Parameters:
        //   id - The ID of the product you’re looking for.
        // Returns the Product object if found, or null if it doesn’t exist.
        Task<Product> GetProductByIdAsync(int id);

        // Retrieves detailed information for a specific product by its unique ID (ProductId).
        // Parameters:
        //   id - The ID of the product you’re looking for.
        // Returns a ProductDTO containing detailed information about the product, or null if it doesn’t exist.
        Task<ProductDTO> GetProductDTOByIdAsync(int id);

        // Adds a new product to the database.
        // Parameters:
        //   product - The Product object containing the details of the new product.
        // Saves the product to the database.
        Task AddProductAsync(Product product);

        // Updates an existing product’s details in the database.
        // Parameters:
        //   product - The Product object with the updated information.
        // Saves the changes to the database.
        Task UpdateProductAsync(Product product);

        // Checks if a product exists in the database by its unique ID (ProductId).
        // Parameters:
        //   id - The ID of the product you want to check.
        // Returns true if the product exists, or false if it doesn’t.
        Task<bool> ProductExistsAsync(int id);

        // Retrieves all unique product categories from the database.
        // Returns a collection of strings representing the available product categories.
        Task<IEnumerable<string>> GetAllCategoriesAsync();
    }
}