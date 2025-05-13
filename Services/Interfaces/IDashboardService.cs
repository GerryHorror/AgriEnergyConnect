using AgriEnergyConnect.Models.ViewModels;

namespace AgriEnergyConnect.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<EmployeeDashboardViewModel> GetDashboardDataAsync(int userId);
    }
}