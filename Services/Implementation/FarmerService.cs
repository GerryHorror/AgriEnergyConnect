using AgriEnergyConnect.DTOs;
using AgriEnergyConnect.Helpers;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Models.ViewModels;
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

        public async Task<Farmer> CreateFarmerFromViewModelAsync(FarmerViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Create user identity
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Role = UserRole.Farmer,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            // Create farmer entity
            var farmer = new Farmer
            {
                FarmName = model.FarmName,
                Location = model.Location,
                User = user
            };

            // Add the farmer (this method already exists and handles password hashing)
            await AddFarmerAsync(farmer, model.Password);

            return farmer;
        }

        public async Task<(List<FarmerSummaryDTO> Farmers, int TotalPages, int TotalFarmers, List<string> UniqueLocations)>
    GetFilteredFarmersAsync(string searchTerm, string locationFilter, string statusFilter, int page, int pageSize)
        {
            // Get all farmers with their complete information
            var farmers = await GetAllFamersAsync();
            var farmerList = farmers.ToList();

            // Create farmer summaries with correct IsActive and ProductCount
            var farmerSummaries = farmerList.Select(farmer => new FarmerSummaryDTO
            {
                FarmerId = farmer.FarmerId,
                FarmName = farmer.FarmName,
                Location = farmer.Location,
                OwnerName = farmer.User != null
                    ? $"{farmer.User.FirstName} {farmer.User.LastName}".Trim()
                    : "Unknown Farmer",
                ProductCount = farmer.Products?.Count ?? 0,
                IsActive = farmer.User?.IsActive ?? false
            }).ToList();

            // Apply status filter
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "active")
                {
                    farmerSummaries = farmerSummaries.Where(f => f.IsActive).ToList();
                }
                else if (statusFilter == "inactive")
                {
                    farmerSummaries = farmerSummaries.Where(f => !f.IsActive).ToList();
                }
            }

            // Apply other filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                farmerSummaries = farmerSummaries.Where(f =>
                    f.OwnerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    f.FarmName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    f.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(locationFilter))
            {
                farmerSummaries = farmerSummaries.Where(f =>
                    f.Location.Equals(locationFilter, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Get unique locations for the filter dropdown
            var uniqueLocations = farmerList.Select(f => f.Location)
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            // Implement pagination
            var totalFarmers = farmerSummaries.Count;
            var totalPages = (int)Math.Ceiling(totalFarmers / (double)pageSize);
            var paginatedFarmers = farmerSummaries
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (paginatedFarmers, totalPages, totalFarmers, uniqueLocations);
        }

        public async Task<FarmerDTO> GetFarmerDetailsForViewAsync(int farmerId)
        {
            var farmer = await _farmerRepository.GetFarmerByIdAsync(farmerId);

            if (farmer == null)
                throw new KeyNotFoundException($"Farmer with ID {farmerId} not found");

            // Convert to DTO which includes related information
            var farmerDTO = MappingHelper.ToFarmerDTO(farmer);

            return farmerDTO;
        }

        public async Task<Farmer> UpdateFarmerFromViewModelAsync(int farmerId, FarmerViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Get the existing farmer
            var farmer = await _farmerRepository.GetFarmerByIdAsync(farmerId);
            if (farmer == null)
                throw new KeyNotFoundException($"Farmer with ID {farmerId} not found");

            // Update farmer information
            farmer.FarmName = model.FarmName;
            farmer.Location = model.Location;

            // Update user information
            if (farmer.User != null)
            {
                farmer.User.FirstName = model.FirstName;
                farmer.User.LastName = model.LastName;
                farmer.User.Email = model.Email;
                farmer.User.PhoneNumber = model.PhoneNumber;
                farmer.User.Username = model.Username;

                // Only update password if a new one is provided
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var hasher = new PasswordHasher<User>();
                    farmer.User.PasswordHash = hasher.HashPassword(farmer.User, model.Password);
                }
            }
            else
            {
                throw new InvalidOperationException("Farmer's user information is missing");
            }

            // Save changes
            await UpdateFarmerAsync(farmer);

            return farmer;
        }

        public async Task<FarmerViewModel> GetFarmerForEditingAsync(int farmerId)
        {
            var farmer = await _farmerRepository.GetFarmerByIdAsync(farmerId);
            if (farmer == null)
                throw new KeyNotFoundException($"Farmer with ID {farmerId} not found");

            // Map farmer entity to view model
            var viewModel = new FarmerViewModel
            {
                FarmName = farmer.FarmName,
                Location = farmer.Location,
                FirstName = farmer.User?.FirstName,
                LastName = farmer.User?.LastName,
                Email = farmer.User?.Email,
                PhoneNumber = farmer.User?.PhoneNumber,
                Username = farmer.User?.Username
                // Password is not mapped since we don't want to expose it
            };

            return viewModel;
        }

        public async Task DeactivateFarmerAsync(int farmerId)
        {
            var farmer = await _farmerRepository.GetFarmerByIdAsync(farmerId);
            if (farmer == null)
                throw new KeyNotFoundException($"Farmer with ID {farmerId} not found");

            if (farmer.User == null)
                throw new InvalidOperationException("Farmer's user information is missing");

            // Deactivate the user
            farmer.User.IsActive = false;

            // Save changes
            await UpdateFarmerAsync(farmer);
        }

        public async Task ReactivateFarmerAsync(int farmerId)
        {
            var farmer = await _farmerRepository.GetFarmerByIdAsync(farmerId);
            if (farmer == null)
                throw new KeyNotFoundException($"Farmer with ID {farmerId} not found");

            if (farmer.User == null)
                throw new InvalidOperationException("Farmer's user information is missing");

            // Reactivate the user
            farmer.User.IsActive = true;

            // Save changes
            await UpdateFarmerAsync(farmer);
        }
    }
}