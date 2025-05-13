using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Services.Interfaces
{
    public interface IFarmerDashboardService
    {
        Task<(Farmer Farmer, int TotalProducts, int CategoryCount, string LastAddedTime, List<string> CategoryList, List<Product> RecentProducts, List<ActivityItem> RecentActivities, DateTime? LastProductAdded)> GetDashboardDataAsync(int userId);
    }
}