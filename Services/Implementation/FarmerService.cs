using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Helpers;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Repositories.Interfaces;
using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AgriEnergyConnect.Services.Implementation
{
    // The FarmerService class implements the IFarmerService interface and provides the business logic
    // for managing farmers in the system. It interacts with the repository layer to perform CRUD operations.
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        // Constructor that accepts the IFarmerRepository to interact with the database.
        // Dependency injection is used to provide the repository.
        public FarmerService(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }

        // Retrieves all farmers from the database.
        // Returns a collection of Farmer objects.
        public async Task<IEnumerable<Farmer>> GetAllFamersAsync()
        {
            return await _farmerRepository.GetAllFarmersAsync();
        }

        // Retrieves summarised information for all farmers.
        // Returns a collection of FarmerSummaryDTO objects, typically used for listing or preview purposes.
        public async Task<IEnumerable<FarmerSummaryDTO>> GetAllFarmerSummariesAsync()
        {
            var farmers = await _farmerRepository.GetAllFarmersAsync();
            return MappingHelper.ToFarmerSummaryDTOList(farmers);
        }

        // Retrieves detailed information for a specific farmer by their unique ID (FarmerId).
        // Parameters:
        //   id - The ID of the farmer you’re looking for.
        // Returns a FarmerDTO containing detailed information about the farmer, or null if it doesn’t exist.
        public async Task<FarmerDTO> GetFarmerDTOByIdAsync(int id)
        {
            var farmer = await _farmerRepository.GetFarmerByIdAsync(id);
            return MappingHelper.ToFarmerDTO(farmer);
        }

        // Retrieves a specific farmer by their unique ID (FarmerId).
        // Parameters:
        //   id - The ID of the farmer you’re looking for.
        // Returns the Farmer object if found, or null if it doesn’t exist.
        public async Task<Farmer> GetFarmerByIdAsync(int id)
        {
            return await _farmerRepository.GetFarmerByIdAsync(id);
        }

        // Retrieves detailed information for a specific farmer by the UserId of the associated user account.
        // Parameters:
        //   userId - The ID of the user linked to the farmer.
        // Returns a FarmerDTO containing detailed information about the farmer, or null if it doesn’t exist.
        public async Task<FarmerDTO> GetFarmerDTOByUserIdAsync(int userId)
        {
            var farmer = await _farmerRepository.GetFarmerByUserIdAsync(userId);
            return MappingHelper.ToFarmerDTO(farmer);
        }

        // Retrieves a specific farmer by the UserId of the associated user account.
        // Parameters:
        //   userId - The ID of the user linked to the farmer.
        // Returns the Farmer object if found, or null if it doesn’t exist.
        public async Task<Farmer> GetFarmerByUserIdAsync(int userId)
        {
            return await _farmerRepository.GetFarmerByUserIdAsync(userId);
        }

        // Adds a new farmer to the database.
        // Hashes the password for the associated user account and ensures the user is set to the Farmer role.
        // Parameters:
        //   farmer - The Farmer object containing the details of the new farmer.
        //   password - The password for the associated user account.
        // Returns the newly created Farmer object.
        public async Task<Farmer> AddFarmerAsync(Farmer farmer, string password)
        {
            // Hash the password before storing it.
            farmer.User.PasswordHash = _passwordHasher.HashPassword(farmer.User, password);

            // Ensure the user is set to the Farmer role.
            farmer.User.Role = UserRole.Farmer;

            await _farmerRepository.AddFarmerAsync(farmer);
            return farmer;
        }

        // Updates an existing farmer’s details in the database.
        // Parameters:
        //   farmer - The Farmer object with the updated information.
        // Saves the changes to the database.
        public async Task UpdateFarmerAsync(Farmer farmer)
        {
            await _farmerRepository.UpdateFarmerAsync(farmer);
        }

        // Checks if a farmer exists in the database by their unique ID (FarmerId).
        // Parameters:
        //   id - The ID of the farmer you want to check.
        // Returns true if the farmer exists, or false if they don’t.
        public async Task<bool> FarmerExistsAsync(int id)
        {
            return await _farmerRepository.FarmerExistsAsync(id);
        }
    }
}