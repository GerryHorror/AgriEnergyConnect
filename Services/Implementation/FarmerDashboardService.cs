using AgriEnergyConnect.Models;
using AgriEnergyConnect.Services.Interfaces;

namespace AgriEnergyConnect.Services.Implementation
{
    public class FarmerDashboardService : IFarmerDashboardService
    {
        private readonly IFarmerService _farmerService;
        private readonly IProductService _productService;

        public FarmerDashboardService(IFarmerService farmerService, IProductService productService)
        {
            _farmerService = farmerService;
            _productService = productService;
        }

        public async Task<(Farmer Farmer, int TotalProducts, int CategoryCount, string LastAddedTime,
                         List<string> CategoryList, List<Product> RecentProducts, List<ActivityItem> RecentActivities,
                         DateTime? LastProductAdded)> GetDashboardDataAsync(int userId)
        {
            // Get farmer data
            var farmer = await _farmerService.GetFarmerByUserIdAsync(userId);
            if (farmer == null)
                throw new KeyNotFoundException($"Farmer with user ID {userId} not found");

            // Get all product DTOs for the farmer
            var productDTOs = await _productService.GetProductDTOsByFarmerIdAsync(farmer.FarmerId);
            var productList = productDTOs.ToList();

            // Calculate statistics
            var totalProducts = productList.Count;
            var categoryCount = productList.Select(p => p.Category).Distinct().Count();
            var categoryList = productList.Select(p => p.Category).Distinct().ToList();

            // Get the last added product time
            var lastProduct = productList.OrderByDescending(p => p.CreatedDate).FirstOrDefault();
            DateTime? lastProductDate = lastProduct?.CreatedDate;
            string lastAddedTime;

            if (lastProduct != null)
            {
                var timeSpan = DateTime.Now - lastProduct.CreatedDate;
                lastAddedTime = FormatTimeAgo(timeSpan);
            }
            else
            {
                lastAddedTime = "No products yet";
            }

            // Convert ProductDTOs to Products for view compatibility
            var recentProducts = ConvertProductDTOsToProducts(
                productList.OrderByDescending(p => p.CreatedDate).Take(5).ToList());

            // Create recent activities
            var recentActivities = GetRecentActivitiesFromDTOs(productList);

            return (farmer, totalProducts, categoryCount, lastAddedTime, categoryList,
                   recentProducts, recentActivities, lastProductDate);
        }

        // Helper method to format time ago
        private string FormatTimeAgo(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 365)
            {
                int years = (int)(timeSpan.TotalDays / 365);
                return $"{years} year{(years > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalDays >= 30)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return $"{months} month{(months > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalDays >= 1)
            {
                int days = (int)timeSpan.TotalDays;
                return $"{days} day{(days > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalHours >= 1)
            {
                int hours = (int)timeSpan.TotalHours;
                return $"{hours} hour{(hours > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalMinutes >= 1)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                return $"{minutes} minute{(minutes > 1 ? "s" : "")} ago";
            }
            return "just now";
        }

        // Helper method to convert ProductDTOs to Product entities for view compatibility
        private List<Product> ConvertProductDTOsToProducts(List<AgriEnergyConnect.DTOs.ProductDTO> productDTOs)
        {
            return productDTOs.Select(dto => new Product
            {
                ProductId = dto.ProductId,
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                ProductionDate = dto.ProductionDate,
                FarmerId = dto.FarmerId,
                CreatedDate = dto.CreatedDate,
                IsActive = dto.IsActive
            }).ToList();
        }

        // Helper method to generate recent activities from ProductDTOs
        private List<ActivityItem> GetRecentActivitiesFromDTOs(List<AgriEnergyConnect.DTOs.ProductDTO> productDTOs)
        {
            var activities = new List<ActivityItem>();

            // Get all products ordered by creation date to show actual activity
            var sortedProducts = productDTOs.OrderByDescending(p => p.CreatedDate).ToList();

            // Add activity items for each product creation
            foreach (var product in sortedProducts.Take(5))  // Limit to 5 activities
            {
                activities.Add(new ActivityItem
                {
                    Description = $"Added \"{product.Name}\"",
                    Time = FormatTimeAgo(DateTime.Now - product.CreatedDate),
                    Color = "#4CAF50",
                    Icon = "fa-plus"
                });
            }

            return activities;
        }
    }
}