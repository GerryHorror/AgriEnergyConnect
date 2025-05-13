using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Repositories.Interfaces
{
    // This interface defines the methods for working with RegistrationRequest data.
    // It's like a contract that ensures any class implementing it will provide these functionalities.
    public interface IRegistrationRequestRepository
    {
        // Fetches all registration requests from the database.
        // Returns a collection of RegistrationRequest objects.
        Task<IEnumerable<RegistrationRequest>> GetAllRequestsAsync();

        // Fetches all pending registration requests from the database.
        // Returns a collection of RegistrationRequest objects that have a status of "Pending".
        Task<IEnumerable<RegistrationRequest>> GetPendingRequestsAsync();

        // Retrieves a specific registration request by its unique ID (RequestId).
        // Parameters:
        //   requestId - The ID of the registration request to retrieve.
        // Returns the RegistrationRequest object if found, or null if it doesn't exist.
        Task<RegistrationRequest> GetRequestByIdAsync(int requestId);

        // Adds a new registration request to the database.
        // Parameters:
        //   request - The RegistrationRequest object containing the details of the new request.
        // Saves the registration request to the database asynchronously.
        Task AddRequestAsync(RegistrationRequest request);

        // Updates an existing registration request in the database.
        // Parameters:
        //   request - The RegistrationRequest object with the updated information.
        // Saves the changes to the database asynchronously.
        Task UpdateRequestAsync(RegistrationRequest request);

        // Deletes a registration request from the database.
        // Parameters:
        //   requestId - The ID of the registration request to delete.
        // Removes the registration request from the database asynchronously.
        Task DeleteRequestAsync(int requestId);

        // Checks if a registration request exists for a specific username or email.
        // Parameters:
        //   username - The username to check.
        //   email - The email to check.
        // Returns true if a request exists for the given username or email, or false otherwise.
        Task<bool> RequestExistsAsync(string username, string email);
    }
}