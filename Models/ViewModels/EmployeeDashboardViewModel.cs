using AgriEnergyConnect.DTOs;

namespace AgriEnergyConnect.Models.ViewModels
{
    // ViewModel for the Employee Dashboard page
    public class EmployeeDashboardViewModel
    {
        // Employee information
        public User Employee { get; set; }

        // Dashboard statistics
        public int TotalFarmers { get; set; }

        public int TotalProducts { get; set; }
        public int ActiveUsers { get; set; }
        public int NewFarmers { get; set; }
        public int NewProducts { get; set; }
        public decimal ActivePercentage { get; set; }

        // Recent farmers list
        public List<FarmerSummaryDTO> RecentFarmers { get; set; } = new List<FarmerSummaryDTO>();

        // Activity feed - using the ActivityItem class that has the static methods
        public List<ActivityItem> RecentActivities { get; set; } = new List<ActivityItem>();
    }
}