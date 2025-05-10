// ActivityItem class for dashboard activities
public class ActivityItem
{
    public string Description { get; set; }
    public string Time { get; set; }
    public string Color { get; set; }
    public string Icon { get; set; }
    public string Title { get; set; }

    // Static methods to create common activities for Employee dashboard
    public static ActivityItem NewFarmerAdded(string farmerName, DateTime addedTime)
    {
        return new ActivityItem
        {
            Title = "New Farmer Added",
            Description = $"{farmerName} was added to the system",
            Time = FormatTime(addedTime),
            Color = "#00897B",
            Icon = "fa-plus"
        };
    }

    public static ActivityItem ProductUpdate(string farmerName, int productCount, DateTime updateTime)
    {
        return new ActivityItem
        {
            Title = "Product Update",
            Description = $"{productCount} new products added by {farmerName}",
            Time = FormatTime(updateTime),
            Color = "#FF9800",
            Icon = "fa-edit"
        };
    }

    public static ActivityItem ReportGenerated(string reportType, DateTime generatedTime)
    {
        return new ActivityItem
        {
            Title = "Report Generated",
            Description = $"{reportType} analysis complete",
            Time = FormatTime(generatedTime),
            Color = "#7CB342",
            Icon = "fa-file-alt"
        };
    }

    // Helper method to format time
    private static string FormatTime(DateTime time)
    {
        var timeAgo = DateTime.Now - time;

        if (timeAgo.TotalMinutes < 1)
            return "Just now";
        if (timeAgo.TotalHours < 1)
            return $"{(int)timeAgo.TotalMinutes} minutes ago";
        if (timeAgo.TotalDays < 1)
            return $"Today, {time:hh:mm tt}";
        if (timeAgo.TotalDays < 2)
            return $"Yesterday, {time:hh:mm tt}";
        if (timeAgo.TotalDays < 7)
            return $"{(int)timeAgo.TotalDays} days ago";
        return time.ToString("MMM d, yyyy");
    }
}