namespace AgriEnergyConnect.DTOs
{
    // The ProductGroupData class is a DTO for summarising product group information for a farmer.
    public class ProductGroupData
    {
        // The unique identifier of the farmer.
        public int FarmerId { get; set; }

        // The number of products in the group.
        public int Count { get; set; }

        // The date and time when the group was last updated.
        public DateTime LastUpdate { get; set; }
    }
}