namespace AgriEnergyConnect.DTOs
{
    // The FarmerDTO class is a Data Transfer Object (DTO) used to transfer detailed farmer information
    // between the application layers. It includes farmer details, user information, and additional metadata.
    public class FarmerDTO
    {
        // The unique identifier for the farmer.
        public int FarmerId { get; set; }

        // The name of the farm associated with the farmer.
        public string FarmName { get; set; }

        // The location of the farm (e.g., city, region, or address).
        public string Location { get; set; }

        // User details

        // The unique identifier of the user associated with the farmer.
        public int UserId { get; set; }

        // The username of the user associated with the farmer.
        public string Username { get; set; }

        // The first name of the user associated with the farmer.
        public string FirstName { get; set; }

        // The last name of the user associated with the farmer.
        public string LastName { get; set; }

        // The email address of the user associated with the farmer.
        public string Email { get; set; }

        // The phone number of the user associated with the farmer.
        public string PhoneNumber { get; set; }

        // The date when the farmer joined the system.
        public DateTime JoinedDate { get; set; }

        // Additional information

        // The total number of products associated with the farmer.
        public int ProductCount { get; set; }

        // Indicates whether the farmer is currently active in the system.
        public bool IsActive { get; set; }

        // A list of recent products associated with the farmer.
        // This is a summary of the farmer's products, typically used for display purposes.
        public List<ProductSummaryDTO> RecentProducts { get; set; }
    }

    // The FarmerSummaryDTO class is a lightweight Data Transfer Object (DTO) used to transfer
    // summarised farmer information, typically for listing or preview purposes.
    public class FarmerSummaryDTO
    {
        // The unique identifier for the farmer.
        public int FarmerId { get; set; }

        // The name of the farm associated with the farmer.
        public string FarmName { get; set; }

        // The location of the farm (e.g., city, region, or address).
        public string Location { get; set; }

        // The full name of the farmer (e.g., "John Smith").
        public string OwnerName { get; set; }

        // The total number of products associated with the farmer.
        public int ProductCount { get; set; }

        // Indicates whether the farmer is currently active in the system.
        public bool IsActive { get; set; }
    }
}