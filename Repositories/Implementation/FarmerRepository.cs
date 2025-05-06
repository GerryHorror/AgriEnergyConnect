using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.Repositories.Implementation
{
    // This class implements the IFarmerRepository interface and provides the actual logic
    // for interacting with the Farmer data in the database.
    public class FarmerRepository : IFarmerRepository
    {
        private readonly AppDbContext _context;

        // Constructor that accepts the AppDbContext to interact with the database.
        // Dependency injection is used to provide the context.
        public FarmerRepository(AppDbContext context)
        {
            _context = context;
        }

        // Fetches all farmers from the database, including their associated User details.
        // Returns a list of Farmer objects.
        public async Task<IEnumerable<Farmer>> GetAllFarmersAsync()
        {
            return await _context.Farmers
                .Include(f => f.User) // Includes the related User entity for each Farmer.
                .ToListAsync(); // Converts the result to a list asynchronously.
        }

        // Finds a specific farmer by their unique ID (FarmerId).
        // Includes the associated User and Products details.
        // Parameters:
        //   id - The ID of the farmer you’re looking for.
        // Returns the Farmer object if found, or null if it doesn’t exist.
        public async Task<Farmer> GetFarmerByIdAsync(int id)
        {
            return await _context.Farmers
                .Include(f => f.User) // Includes the related User entity.
                .Include(f => f.Products) // Includes the related Products collection.
                .FirstOrDefaultAsync(f => f.FarmerId == id); // Finds the first match or returns null.
        }

        // Finds a farmer based on the UserId of the associated user account.
        // Parameters:
        //   userId - The ID of the user linked to the farmer.
        // Returns the Farmer object if found, or null if it doesn’t exist.
        public async Task<Farmer> GetFarmerByUserIdAsync(int userId)
        {
            return await _context.Farmers
                .Include(f => f.User) // Includes the related User entity.
                .FirstOrDefaultAsync(f => f.UserId == userId); // Finds the first match or returns null.
        }

        // Adds a new farmer to the database.
        // Parameters:
        //   farmer - The Farmer object containing the details of the new farmer.
        // Saves the farmer to the database asynchronously.
        public async Task AddFarmerAsync(Farmer farmer)
        {
            await _context.Farmers.AddAsync(farmer); // Adds the farmer to the context.
            await _context.SaveChangesAsync(); // Saves the changes to the database.
        }

        // Updates an existing farmer’s details in the database.
        // Parameters:
        //   farmer - The Farmer object with the updated information.
        // Saves the changes to the database asynchronously.
        public async Task UpdateFarmerAsync(Farmer farmer)
        {
            _context.Farmers.Update(farmer); // Marks the farmer entity as updated.
            await _context.SaveChangesAsync(); // Saves the changes to the database.
        }

        // Checks if a farmer exists in the database by their unique ID (FarmerId).
        // Parameters:
        //   id - The ID of the farmer you want to check.
        // Returns true if the farmer exists, or false if they don’t.
        public async Task<bool> FarmerExistsAsync(int id)
        {
            return await _context.Farmers
                .AnyAsync(f => f.FarmerId == id); // Checks if any farmer matches the given ID.
        }
    }
}