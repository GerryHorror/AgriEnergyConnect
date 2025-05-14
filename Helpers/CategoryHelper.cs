namespace AgriEnergyConnect.Helpers
{
    public static class CategoryHelper
    {
        public static readonly List<CategoryInfo> Categories = new()
        {
            new CategoryInfo("Grains", "#E1F5FE"),
            new CategoryInfo("Vegetables", "#F1F8E9"),
            new CategoryInfo("Fruits", "#FFFDE7"),
            new CategoryInfo("Herbs", "#E8F5E9"),
            new CategoryInfo("Roots", "#FBE9E7"),
            new CategoryInfo("Nuts", "#FFF3E0"),
            new CategoryInfo("Dairy", "#FFF3E0"),
            new CategoryInfo("Meat", "#FFEBEE"),
            new CategoryInfo("Eggs", "#FFFDE7"),
            new CategoryInfo("Poultry", "#F3E5F5"),
            new CategoryInfo("Fish", "#E3F2FD"),
            new CategoryInfo("Honey", "#FFFDE7"),
            new CategoryInfo("Livestock", "#E8F5E9"),
            new CategoryInfo("Organic", "#E8F5E9"),
            new CategoryInfo("Mushrooms", "#F3E5F5"),
            new CategoryInfo("Beverages", "#E1F5FE"),
            new CategoryInfo("Wool", "#F5F5F5"),
            new CategoryInfo("Compost", "#E0F2F1"),
            new CategoryInfo("Seeds", "#FFFDE7"),
            new CategoryInfo("Saplings", "#E8F5E9"),
            new CategoryInfo("Feed", "#FFF3E0"),
            new CategoryInfo("Fertilisers", "#FBE9E7"),
            new CategoryInfo("Biogas", "#E0F7FA"),
        };

        public static string GetCategoryColor(string category)
        {
            var cat = Categories.FirstOrDefault(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
            return cat?.Color ?? "#F5F5F5";
        }
    }

    public class CategoryInfo
    {
        public string Name { get; }
        public string Color { get; }

        public CategoryInfo(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }
}