using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using AgriEnergyConnect.Models.ViewModels;
using AgriEnergyConnect.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public async Task<User> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Check if username already exists
            if (await UsernameExistsAsync(model.Username))
                throw new InvalidOperationException("Username already exists");

            // Check if email already exists
            if (await EmailExistsAsync(model.Email))
                throw new InvalidOperationException("Email already exists");

            // Create the user
            var user = new User
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = model.IsFarmer ? UserRole.Farmer : UserRole.Employee,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            // Hash the password
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            // Add user to database
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // If registering as a farmer, create farmer entity
            if (model.IsFarmer)
            {
                var farmer = new Farmer
                {
                    FarmName = model.FarmName,
                    Location = model.Location,
                    UserId = user.UserId,
                    User = user
                };

                await _context.Farmers.AddAsync(farmer);
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public ClaimsIdentity CreateUserClaims(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Create claims for the authenticated user
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
    };

            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}