using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Models.ViewModels;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Services.Interfaces;

namespace AgriEnergyConnect.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IFarmerService _farmerService;
        private readonly IProductService _productService;
        private readonly IAuthService _authService;

        public DashboardService(
            IFarmerService farmerService,
            IProductService productService,
            IAuthService authService)
        {
            _farmerService = farmerService;
            _productService = productService;
            _authService = authService;
        }

        public async Task<EmployeeDashboardViewModel> GetDashboardDataAsync(int userId)
        {
            // Get user data
            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Get all farmers and products for statistics
            var farmers = await _farmerService.GetAllFamersAsync();
            var farmerList = farmers.ToList();
            var products = await _productService.GetAllProductsAsync();
            var productList = products.ToList();
            var allUsers = await _authService.GetAllUsersAsync();
            var userList = allUsers.ToList();

            // Calculate statistics
            var totalFarmers = farmerList.Count;
            var totalProducts = productList.Count;
            var activeUsers = userList.Count(u => u.IsActive);
            var totalUsers = userList.Count;

            // Get all farmers for "Recent Farmers" section (not just recent additions)
            var recentFarmers = farmerList
                .OrderByDescending(f => f.User?.CreatedDate ?? DateTime.MinValue)
                .Take(4)
                .ToList();

            // Get recent products (last 7 days)
            var recentProducts = productList
                .Where(p => p.CreatedDate >= DateTime.Now.AddDays(-7))
                .GroupBy(p => p.FarmerId)
                .Select(g => new ProductGroupData
                {
                    FarmerId = g.Key,
                    Count = g.Count(),
                    LastUpdate = g.Max(p => p.CreatedDate)
                })
                .ToList();

            // Calculate additional statistics
            var newFarmers = recentFarmers.Count(f => f.User?.CreatedDate >= DateTime.Now.AddDays(-7));
            var newProducts = recentProducts.Sum(p => p.Count);
            var activePercentage = totalUsers > 0 ? Math.Round((decimal)activeUsers / totalUsers * 100, 1) : 0;

            // Create recent activities
            var activities = CreateRecentActivities(recentFarmers, recentProducts, farmerList);

            // Create the view model
            var viewModel = new EmployeeDashboardViewModel
            {
                Employee = user,
                TotalFarmers = totalFarmers,
                TotalProducts = totalProducts,
                ActiveUsers = activeUsers,
                NewFarmers = newFarmers,
                NewProducts = newProducts,
                ActivePercentage = activePercentage,
                // Map farmers to FarmerSummaryDTO
                RecentFarmers = recentFarmers.Select(f => new FarmerSummaryDTO
                {
                    FarmerId = f.FarmerId,
                    FarmName = f.FarmName,
                    Location = f.Location,
                    OwnerName = f.User != null ? $"{f.User.FirstName ?? "Unknown"} {f.User.LastName ?? "Farmer"}".Trim() : "Unknown Farmer",
                    ProductCount = f.Products?.Count ?? 0
                }).ToList(),
                RecentActivities = activities
            };

            return viewModel;
        }

        private List<ActivityItem> CreateRecentActivities(List<Farmer> newFarmers, List<ProductGroupData> recentProducts, List<Farmer> farmerList)
        {
            var activities = new List<ActivityItem>();

            // Add recent farmer additions (for the past week)
            var recentNewFarmers = newFarmers.Where(f => f.User?.CreatedDate >= DateTime.Now.AddDays(-7)).Take(1);
            foreach (var farmer in recentNewFarmers)
            {
                if (farmer.User != null)
                {
                    activities.Add(ActivityItem.NewFarmerAdded(
                        $"{farmer.User.FirstName} {farmer.User.LastName}",
                        farmer.User.CreatedDate
                    ));
                }
            }

            // Add recent product updates
            foreach (var productGroup in recentProducts.Take(1))
            {
                var farmer = farmerList.FirstOrDefault(f => f.FarmerId == productGroup.FarmerId);
                if (farmer?.User != null)
                {
                    activities.Add(ActivityItem.ProductUpdate(
                        $"{farmer.User.FirstName} {farmer.User.LastName}",
                        productGroup.Count,
                        productGroup.LastUpdate
                    ));
                }
            }

            // Add a sample report activity
            activities.Add(ActivityItem.ReportGenerated(
                "Monthly product",
                DateTime.Now.AddDays(-1).AddHours(-7).AddMinutes(-20)
            ));

            return activities;
        }
    }
}