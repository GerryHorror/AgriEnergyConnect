using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergyConnect.Services.Implementation
{
    // The AuthService class provides the implementation of the IAuthService interface.
    // It handles user authentication, retrieval, and validation of user-related data.
    public class AuthService : IAuthService
    {
        // The database context used to access the application's data.
        private readonly AppDbContext _context;

        // The PasswordHasher is used to securely hash and verify user passwords.
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        // Constructor for the AuthService class.
        // Parameters:
        //   context - The database context used to access the application's data.
        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // Authenticates a user based on their username and password.
        // Parameters:
        //   username - The username of the user attempting to authenticate.
        //   password - The password of the user attempting to authenticate.
        // Returns a User object if authentication is successful, or null if authentication fails.
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            // Find the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            // Check if the user exists
            if (user == null)
                return null;

            // Verify the password
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Success)
            {
                // Update the last login date
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return user;
            }

            // Authentication failed
            return null;
        }

        // Retrieves a user by their unique identifier.
        // Parameters:
        //   id - The unique identifier of the user to retrieve.
        // Returns a User object if a user with the specified ID exists, or null if no such user is found.
        public async Task<User> GetUserByIdAsync(int id)
        {
            // Find the user by ID
            return await _context.Users.FindAsync(id);
        }

        // Retrieves a user by their username.
        // Parameters:
        //   username - The username of the user to retrieve.
        // Returns a User object if a user with the specified username exists, or null if no such user is found.
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            // Find the user by username
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        // Checks if a username already exists in the system.
        // Parameters:
        //   username - The username to check for existence.
        // Returns true if the username exists, or false otherwise.
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        // Checks if an email address already exists in the system.
        // Parameters:
        //   email - The email address to check for existence.
        // Returns true if the email address exists, or false otherwise.
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Retrieves all users in the system.
        // Returns a list of User objects representing all users.
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Retrieve all users from the database
            return await _context.Users.ToListAsync();
        }
    }
}