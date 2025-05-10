namespace AgriEnergyConnect.Models.ViewModels
{
    public class FarmerProductsViewModel
    {
        public string FarmName { get; set; }
        public int TotalProducts { get; set; }

        public List<ProductViewModel> Products { get; set; } = new();

        // Filter fields

        public string SearchTerm { get; set; }
        public string SelectedCategory { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
    }
}