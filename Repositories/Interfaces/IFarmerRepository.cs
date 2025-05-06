using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Repositories.Interfaces
{
    // This interface defines the methods for working with Farmer data.
    // It’s like a contract that ensures any class implementing it will provide these functionalities.
    public interface IFarmerRepository
    {
        // Fetches all the farmers from the database.
        // Returns a list of Farmer objects.
        Task<IEnumerable<Farmer>> GetAllFarmersAsync();

        // Finds a specific farmer by their unique ID (FarmerId).
        // Parameters:
        //   id - The ID of the farmer you’re looking for.
        // Returns the Farmer object if found, or null if it doesn’t exist.
        Task<Farmer> GetFarmerByIdAsync(int id);

        // Finds a farmer based on the UserId of the associated user account.
        // Parameters:
        //   userId - The ID of the user linked to the farmer.
        // Returns the Farmer object if found, or null if it doesn’t exist.
        Task<Farmer> GetFarmerByUserIdAsync(int userId);

        // Adds a new farmer to the database.
        // Parameters:
        //   farmer - The Farmer object containing the details of the new farmer.
        // Doesn’t return anything, but the farmer will be saved in the database.
        Task AddFarmerAsync(Farmer farmer);

        // Updates an existing farmer’s details in the database.
        // Parameters:
        //   farmer - The Farmer object with the updated information.
        // Doesn’t return anything, but the changes will be saved in the database.
        Task UpdateFarmerAsync(Farmer farmer);

        // Checks if a farmer exists in the database by their unique ID (FarmerId).
        // Parameters:
        //   id - The ID of the farmer you want to check.
        // Returns true if the farmer exists, or false if they don’t.
        Task<bool> FarmerExistsAsync(int id);
    }
}