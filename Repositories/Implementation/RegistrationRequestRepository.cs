using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.Repositories.Implementation
{
    // This class implements the IRegistrationRequestRepository interface and provides the actual logic
    // for interacting with the RegistrationRequest data in the database.
    public class RegistrationRequestRepository : IRegistrationRequestRepository
    {
        private readonly AppDbContext _context;

        // Constructor that accepts the AppDbContext to interact with the database.
        // Dependency injection is used to provide the context.
        public RegistrationRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        // Fetches all registration requests from the database.
        // Returns a list of RegistrationRequest objects ordered by request date (newest first).
        public async Task<IEnumerable<RegistrationRequest>> GetAllRequestsAsync()
        {
            return await _context.RegistrationRequests
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        // Fetches all pending registration requests from the database.
        // Returns a list of RegistrationRequest objects with a status of "Pending" ordered by request date (newest first).
        public async Task<IEnumerable<RegistrationRequest>> GetPendingRequestsAsync()
        {
            return await _context.RegistrationRequests
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        // Retrieves a specific registration request by its unique ID (RequestId).
        // Parameters:
        //   requestId - The ID of the registration request to retrieve.
        // Returns the RegistrationRequest object if found, or null if it doesn't exist.
        public async Task<RegistrationRequest> GetRequestByIdAsync(int requestId)
        {
            return await _context.RegistrationRequests
                .FirstOrDefaultAsync(r => r.RequestId == requestId);
        }

        // Adds a new registration request to the database.
        // Parameters:
        //   request - The RegistrationRequest object containing the details of the new request.
        // Saves the registration request to the database asynchronously.
        public async Task AddRequestAsync(RegistrationRequest request)
        {
            await _context.RegistrationRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        // Updates an existing registration request in the database.
        // Parameters:
        //   request - The RegistrationRequest object with the updated information.
        // Saves the changes to the database asynchronously.
        public async Task UpdateRequestAsync(RegistrationRequest request)
        {
            _context.RegistrationRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        // Deletes a registration request from the database.
        // Parameters:
        //   requestId - The ID of the registration request to delete.
        // Removes the registration request from the database asynchronously.
        public async Task DeleteRequestAsync(int requestId)
        {
            var request = await _context.RegistrationRequests.FindAsync(requestId);

            if (request != null)
            {
                _context.RegistrationRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }

        // Checks if a registration request exists for a specific username or email.
        // Parameters:
        //   username - The username to check.
        //   email - The email to check.
        // Returns true if a request exists for the given username or email, or false otherwise.
        public async Task<bool> RequestExistsAsync(string username, string email)
        {
            return await _context.RegistrationRequests
                .AnyAsync(r =>
                    r.Username.ToLower() == username.ToLower() ||
                    r.Email.ToLower() == email.ToLower());
        }
    }
}