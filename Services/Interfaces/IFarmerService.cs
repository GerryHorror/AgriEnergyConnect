using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Services.Interfaces
{
    // The IFarmerService interface defines the contract for managing farmers in the system.
    // It provides methods for retrieving, adding, updating, and checking the existence of farmers.
    public interface IFarmerService
    {
        // Retrieves all farmers from the database.
        // Returns a collection of Farmer objects.
        Task<IEnumerable<Farmer>> GetAllFamersAsync();

        // Retrieves summarised information for all farmers.
        // Returns a collection of FarmerSummaryDTO objects, typically used for listing or preview purposes.
        Task<IEnumerable<FarmerSummaryDTO>> GetAllFarmerSummaries();

        // Retrieves a specific farmer by their unique ID (FarmerId).
        // Parameters:
        //   id - The ID of the farmer you’re looking for.
        // Returns the Farmer object if found, or null if it doesn’t exist.
        Task<Farmer> GetFarmerByIdAsync(int id);

        // Retrieves detailed information for a specific farmer by their unique ID (FarmerId).
        // Parameters:
        //   id - The ID of the farmer you’re looking for.
        // Returns a FarmerDTO containing detailed information about the farmer, or null if it doesn’t exist.
        Task<FarmerDTO> GetFarmerDTOByIdAsync(int id);

        // Retrieves a specific farmer by the UserId of the associated user account.
        // Parameters:
        //   userId - The ID of the user linked to the farmer.
        // Returns the Farmer object if found, or null if it doesn’t exist.
        Task<Farmer> GetFarmerByUserIdAsync(int userId);

        // Retrieves detailed information for a specific farmer by the UserId of the associated user account.
        // Parameters:
        //   userId - The ID of the user linked to the farmer.
        // Returns a FarmerDTO containing detailed information about the farmer, or null if it doesn’t exist.
        Task<FarmerDTO> GetFarmerDTOByUserIdAsync(int userId);

        // Adds a new farmer to the database.
        // Parameters:
        //   farmer - The Farmer object containing the details of the new farmer.
        //   password - The password for the associated user account.
        // Returns the newly created Farmer object.
        Task<Farmer> AddFarmerAsync(Farmer farmer, string password);

        // Updates an existing farmer’s details in the database.
        // Parameters:
        //   farmer - The Farmer object with the updated information.
        // Saves the changes to the database.
        Task UpdateFarmerAsync(Farmer farmer);

        // Checks if a farmer exists in the database by their unique ID (FarmerId).
        // Parameters:
        //   id - The ID of the farmer you want to check.
        // Returns true if the farmer exists, or false if they don’t.
        Task<bool> FarmerExistsAsync(int id);
    }
}