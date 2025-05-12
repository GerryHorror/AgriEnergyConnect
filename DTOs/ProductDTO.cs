namespace AgriEnergyConnect.DTOs
{
    // The ProductDTO class is a Data Transfer Object (DTO) used to transfer detailed product information
    // between the application layers. It includes product details, farmer information, and additional metadata.
    public class ProductDTO
    {
        // The unique identifier for the product.
        public int ProductId { get; set; }

        // The name of the product (e.g., "Organic Maize").
        public string Name { get; set; }

        // The category of the product (e.g., "Grains", "Vegetables").
        public string Category { get; set; }

        // The date when the product was produced.
        public DateTime ProductionDate { get; set; }

        // A brief description of the product (e.g., "Non-GMO certified").
        public string Description { get; set; }

        // Farmer details

        // The unique identifier of the farmer who produced the product.
        public int FarmerId { get; set; }

        // The name of the farmer who produced the product.
        public string FarmerName { get; set; }

        // The name of the farm where the product was produced.
        public string FarmName { get; set; }

        // Additional information

        // The date and time when the product was created in the system.
        public DateTime CreatedDate { get; set; }

        // Product status
        public bool IsActive { get; set; }
    }

    // The ProductSummaryDTO class is a lightweight Data Transfer Object (DTO) used to transfer
    // summarised product information, typically for listing or preview purposes.
    public class ProductSummaryDTO
    {
        // The unique identifier for the product.
        public int ProductId { get; set; }

        // The name of the product (e.g., "Organic Maize").
        public string Name { get; set; }

        // The category of the product (e.g., "Grains", "Vegetables").
        public string Category { get; set; }

        // The date when the product was produced.
        public DateTime ProductionDate { get; set; }

        // The status of the product (e.g., "Active", "Inactive").
        public bool IsActive { get; set; }
    }

    // The ProductFilterDTO class is a Data Transfer Object (DTO) used to encapsulate filtering criteria
    // for querying products. It allows filtering by farmer, date range, category, or search term.
    public class ProductFilterDTO
    {
        // The unique identifier of the farmer to filter products by.
        // Optional: If null, no filtering by farmer is applied.
        public int? FarmerId { get; set; }

        // The start date of the production date range to filter products by.
        // Optional: If null, no start date filter is applied.
        public DateTime? StartDate { get; set; }

        // The end date of the production date range to filter products by.
        // Optional: If null, no end date filter is applied.
        public DateTime? EndDate { get; set; }

        // The category to filter products by (e.g., "Dairy", "Vegetables").
        // Optional: If null or empty, no category filter is applied.
        public string Category { get; set; }

        // A search term to filter products by name or description.
        // Optional: If null or empty, no search filter is applied.
        public string SearchTerm { get; set; }

        // A flag indicating whether to include only active products in the results.
        // Optional: If null, both active and inactive products are included.
        public bool? ActiveOnly { get; set; }
    }
}