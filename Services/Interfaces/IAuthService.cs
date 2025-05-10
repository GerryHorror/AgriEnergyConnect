using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Services.Interfaces
{
    // The IAuthService interface defines the contract for authentication and user management services.
    // It provides methods for user authentication, retrieval, and validation of user-related data.
    public interface IAuthService
    {
        // Authenticates a user based on their username and password.
        // Parameters:
        //   username - The username of the user attempting to authenticate.
        //   password - The password of the user attempting to authenticate.
        // Returns a User object if authentication is successful, or null if authentication fails.
        Task<User> AuthenticateAsync(string username, string password);

        // Retrieves a user by their unique identifier.
        // Parameters:
        //   id - The unique identifier of the user to retrieve.
        // Returns a User object if a user with the specified ID exists, or null if no such user is found.
        Task<User> GetUserByIdAsync(int id);

        // Retrieves a user by their username.
        // Parameters:
        //   username - The username of the user to retrieve.
        // Returns a User object if a user with the specified username exists, or null if no such user is found.
        Task<User> GetUserByUsernameAsync(string username);

        // Checks if a username already exists in the system.
        // Parameters:
        //   username - The username to check for existence.
        // Returns true if the username exists, or false otherwise.
        Task<bool> UsernameExistsAsync(string username);

        // Checks if an email address already exists in the system.
        // Parameters:
        //   email - The email address to check for existence.
        // Returns true if the email address exists, or false otherwise.
        Task<bool> EmailExistsAsync(string email);

        // Retrieves all users in the system.
        // Returns a list of User objects representing all users.
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}